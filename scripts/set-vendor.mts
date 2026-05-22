// Update osu! entry in lib/package.json
// Usage: `pnpm set-vendor [repository] [revision]`
// Env variables VENDOR_REPOSITORY and VENDOR_REVISION can be used and preferred over arguments.
import PackageJson from '@npmcli/package-json';
import { spawn } from 'node:child_process';
import { once } from 'node:events';
import { convertProcessSignalToExitCode } from 'node:util';

const packageJson = await PackageJson.load('./lib');

// Update osu repository and revision from command line arguments, or use defaults
const repository = process.env.VENDOR_REPOSITORY ?? process.argv[2] ?? 'https://github.com/ppy/osu';
const revision = await resolveRef(repository, process.env.VENDOR_REVISION ?? process.argv[3] ?? 'HEAD');

packageJson.update({
    osu: {
        repository,
        revision,
    }
})

await packageJson.save();
console.log(`Updated osu! vendor settings: repository=${repository}, revision=${revision}`);

async function resolveRef(repo: string, ref: string): Promise<string> {
    const p = spawn('git', ['ls-remote', repo, ref], { stdio: ['inherit', 'pipe', 'inherit'] });

    let output = '';
    p.stdout.setEncoding('utf-8');
    p.stdout.on('data', (data) => output += data);
    
    const [code, signal] = await once(p, 'close');
    const exitCode = code ?? convertProcessSignalToExitCode(signal);
    if (exitCode !== 0) {
        throw new Error(`resolveRef exited with code ${exitCode}`);
    }

    // Return the commit hash from the output, or the original ref if not found
    return output.split('\n')[0].split('\t')[0] || ref;
}
