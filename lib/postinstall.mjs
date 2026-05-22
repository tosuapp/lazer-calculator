// @ts-check
import { spawn } from 'node:child_process';
import packageJSON from './package.json' with { type: 'json' };
import { once } from 'node:events';
import { mkdir, stat } from 'node:fs/promises';

await postinstall();
async function postinstall() {
    try {
        if ((await stat('vendor')).isDirectory()) {
            console.warn('vendor directory already exists, skipping postinstall');
            return;
        }
    } catch {}

    // Fetch osu repository
    await mkdir('vendor', { recursive: true });
    await run('git', ['-C', 'vendor', 'init']);
    await run('git', ['-C', 'vendor', 'remote', 'add', 'origin', packageJSON.osu.repository]);
    await run('git', ['-C', 'vendor', 'fetch', '--depth', '1', 'origin', packageJSON.osu.revision]);
    await run('git', ['-C', 'vendor', 'checkout', 'FETCH_HEAD']);

    // Apply patches
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
