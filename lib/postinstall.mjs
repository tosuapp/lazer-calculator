// @ts-check
import { spawn } from 'node:child_process';
import packageJSON from './package.json' with { type: 'json' };
import { once } from 'node:events';
import { mkdir, access, rm } from 'node:fs/promises';
import { F_OK } from 'node:constants';
import { createInterface } from 'node:readline/promises';

await postinstall();
async function postinstall() {
    if (!checkVendor(packageJSON.osu.revision)) {
        console.warn('vendor is already initialized, skipping postinstall');
        return;
    }
    await rm('vendor', { recursive: true, force: true });

    // Fetch osu repository
    await mkdir('vendor', { recursive: true });
    await run('git', ['-C', 'vendor', 'init']);
    await run('git', ['-C', 'vendor', 'remote', 'add', 'origin', packageJSON.osu.repository]);
    await run('git', ['-C', 'vendor', 'fetch', '--depth', '1', 'origin', packageJSON.osu.revision]);
    await run('git', ['-C', 'vendor', 'checkout', 'FETCH_HEAD']);

    // Create a checkpoint branch for creating patch.
    await run('git', ['-C', 'vendor', 'branch', 'patch-head']);

    // Apply patches
    if (await checkDevMarker()) {
        // Create a checkpoint branch for creating patch.
        await run('git', ['-C', 'vendor', 'branch', 'patch-head']);

        await run(
            'git',
            [
                '-C',
                'vendor',
                'am',
                '--whitespace=fix',
                '-3',
                '../patches/0001-Gradual-diff-calculator.patch',
            ]
        );
    } else {
        // user may not have set identity in production, so use apply instead of am
        await run(
            'git',
            [
                '-C',
                'vendor',
                'apply',
                '--whitespace=fix',
                '-C0',
                '-3',
                '../patches/0001-Gradual-diff-calculator.patch',
            ]
        );
    }
}

/**
 * Check if vendor is already initialized with the given revision.
 * @param {string} revision 
 * @returns {Promise<boolean>}
 */
async function checkVendor(revision) {
    const p = spawn(
        'git',
        ['-C', 'vendor', 'rev-parse', 'patch-head'],
        { cwd: import.meta.dirname }
    );

    const rl = createInterface({
        input: p.stdout,
        terminal: true
    });
    try {
        const line = (await rl[Symbol.asyncIterator]().next()).value;
        if (line !== revision) {
            return false;
        }
    } finally {
        rl.close();
    }

    const [code] = await once(p, 'close');
    return code === 0;
}

/**
 * Run command
 * @param {string} command 
 * @param {string[]} args
 */
async function run(command, args) {
    const p = spawn(command, args, { cwd: import.meta.dirname, stdio: 'inherit' });
    const [code, signal] = await once(p, 'close');

    if (code !== 0) {
        throw new Error(`Command "${command} ${args.join(' ')}" exited with code ${code ?? signal}`);
    }
}

/**
 * Check if .dev-marker file exists, which indicates that the user is in local environment.
 */
async function checkDevMarker() {
    try {
        await access('.dev-marker', F_OK);
        return true;
    } catch {
        return false;
    }
}