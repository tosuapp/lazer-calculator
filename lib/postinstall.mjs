// @ts-check
import { spawn } from 'node:child_process';
import packageJSON from './package.json' with { type: 'json' };
import { once } from 'node:events';
import { mkdir, stat, access } from 'node:fs/promises';
import { F_OK } from 'node:constants';

await postinstall();
async function postinstall() {
    try {
        if ((await stat('vendor')).isDirectory()) {
            console.warn('vendor directory already exists, skipping postinstall');
            return;
        }
    } catch { }

    // Fetch osu repository
    await mkdir('vendor', { recursive: true });
    await run('git', ['-C', 'vendor', 'init']);
    await run('git', ['-C', 'vendor', 'remote', 'add', 'origin', packageJSON.osu.repository]);
    await run('git', ['-C', 'vendor', 'fetch', '--depth', '1', 'origin', packageJSON.osu.revision]);
    await run('git', ['-C', 'vendor', 'checkout', 'FETCH_HEAD']);

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