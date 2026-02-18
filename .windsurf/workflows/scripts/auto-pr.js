const { execSync } = require('child_process');

function exec(command) {
    try {
        return execSync(command, { encoding: 'utf8' }).trim();
    } catch (error) {
        console.error(`Error executing: ${command}`);
        throw error;
    }
}

function log(message, color = 'cyan') {
    const colors = {
        cyan: '\x1b[96m',
        green: '\x1b[92m',
        yellow: '\x1b[93m',
        red: '\x1b[91m',
        reset: '\x1b[0m'
    };
    const timestamp = new Date().toLocaleTimeString();
    console.log(`${colors[color]}[${timestamp}] ${message}${colors.reset}`);
}

function getCurrentBranch() {
    return exec('git branch --show-current');
}

function createFeatureBranch() {
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, -5);
    const branchName = `feature/auto-${timestamp}`;
    log(`Creating branch: ${branchName}`, 'yellow');
    exec(`git checkout -b ${branchName}`);
    return branchName;
}

function stageChanges() {
    log('Staging changes...', 'yellow');
    exec('git add .');
    
    const status = exec('git status --short');
    if (!status) {
        log('No changes to commit', 'red');
        process.exit(1);
    }
    
    log('Changes staged successfully', 'green');
}

function commitChanges(message) {
    if (!message) {
        const status = exec('git status --short');
        const lines = status.split('\n');
        const added = lines.filter(l => l.startsWith('A')).length;
        const modified = lines.filter(l => l.startsWith('M') || l.startsWith(' M')).length;
        const deleted = lines.filter(l => l.startsWith('D')).length;
        
        if (added > 0) {
            message = `feat: Add ${added} new file(s)`;
        } else if (modified > 0) {
            message = `fix: Update ${modified} file(s)`;
        } else if (deleted > 0) {
            message = `chore: Remove ${deleted} file(s)`;
        } else {
            message = 'chore: Update repository';
        }
    }
    
    log(`Committing: ${message}`, 'yellow');
    exec(`git commit -m "${message}"`);
    log('Commit successful', 'green');
    return message;
}

function pushBranch(branchName) {
    log('Pushing to remote...', 'yellow');
    exec(`git push origin ${branchName}`);
    log('Push successful', 'green');
}

function createPR(title, branchName) {
    try {
        log('Creating pull request...', 'yellow');
        
        const body = `## Auto-generated Pull Request

This PR was created automatically using the auto-create-pull-request workflow.

### Changes
- Auto-committed changes from local repository

### Files Changed
${exec(`git diff --name-only origin/main..${branchName}`).split('\n').map(f => `- ${f}`).join('\n')}

### Checklist
- [x] Code committed
- [x] Branch pushed
- [ ] Review required
- [ ] Tests passing`;
        
        exec(`gh pr create --title "${title}" --body "${body.replace(/"/g, '\\"')}" --base main --head ${branchName}`);
        log('✓ Pull request created!', 'green');
        return true;
    } catch (error) {
        log('GitHub CLI not available, generating PR URL...', 'yellow');
        const repoUrl = exec('git config --get remote.origin.url')
            .replace('.git', '')
            .replace('git@github.com:', 'https://github.com/');
        const prUrl = `${repoUrl}/pull/new/${branchName}`;
        console.log(`\nOpen this URL to create PR:\n${prUrl}\n`);
        
        // Try to open in browser (platform-specific)
        try {
            const platform = process.platform;
            if (platform === 'win32') {
                exec(`start ${prUrl}`);
            } else if (platform === 'darwin') {
                exec(`open ${prUrl}`);
            } else {
                exec(`xdg-open ${prUrl}`);
            }
            log('Opened browser to create PR', 'green');
        } catch (e) {
            log('Please manually open the URL above', 'yellow');
        }
        return false;
    }
}

// Main execution
(async () => {
    try {
        console.log('\n');
        log('=== Auto Create Pull Request Workflow ===', 'cyan');
        console.log('\n');
        
        const currentBranch = getCurrentBranch();
        log(`Current branch: ${currentBranch}`, 'cyan');
        
        let branchName = currentBranch;
        if (currentBranch === 'main' || currentBranch === 'master') {
            branchName = createFeatureBranch();
        } else {
            log(`Using existing branch: ${branchName}`, 'cyan');
        }
        
        stageChanges();
        const commitMessage = commitChanges(process.argv[2]);
        pushBranch(branchName);
        const prCreated = createPR(commitMessage, branchName);
        
        console.log('\n');
        log('=== Workflow Complete ===', 'green');
        console.log('\n');
        
        if (prCreated) {
            log('✓ Pull request created and ready for review!', 'green');
        } else {
            log('✓ Branch pushed. Complete PR creation in browser.', 'yellow');
        }
        
    } catch (error) {
        log(`Error: ${error.message}`, 'red');
        process.exit(1);
    }
})();
