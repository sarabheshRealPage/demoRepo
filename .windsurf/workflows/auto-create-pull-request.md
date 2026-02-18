---
description: Automatically create a pull request with minimal manual intervention
---

# Auto Create Pull Request Workflow

This workflow automates the pull request creation process, handling branch creation, commits, and PR submission automatically.

## Prerequisites

- Git repository initialized
- GitHub CLI (`gh`) installed (optional, for full automation)
- Changes ready to commit
- GitHub authentication configured

## Automated Workflow Steps

### Step 1: Detect Current State

The workflow automatically detects:
- Current branch
- Uncommitted changes
- Repository status

### Step 2: Create Feature Branch (if on main)

If currently on `main` or `master`, automatically create a feature branch:

```powershell
# Auto-generate branch name based on timestamp or description
$branchName = "feature/auto-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
git checkout -b $branchName
```

### Step 3: Stage All Changes

Automatically stage all modified and new files:

```powershell
git add .
```

### Step 4: Generate Commit Message

Auto-generate commit message based on changes:

```powershell
# Detect change types
$newFiles = git diff --cached --name-only --diff-filter=A
$modifiedFiles = git diff --cached --name-only --diff-filter=M
$deletedFiles = git diff --cached --name-only --diff-filter=D

# Generate conventional commit message
if ($newFiles.Count -gt 0) {
    $commitType = "feat"
    $description = "Add new files"
} elseif ($modifiedFiles.Count -gt 0) {
    $commitType = "fix"
    $description = "Update existing files"
} else {
    $commitType = "chore"
    $description = "Update repository"
}

$commitMessage = "${commitType}: ${description}"
git commit -m $commitMessage
```

### Step 5: Push to Remote

Automatically push the branch:

```powershell
git push origin $branchName
```

### Step 6: Create Pull Request

#### Option A: Using GitHub CLI (Fully Automated)

```powershell
# Auto-create PR with default template
gh pr create `
    --title "$commitMessage" `
    --body "## Auto-generated Pull Request`n`nThis PR was created automatically.`n`n### Changes`n- Auto-detected changes`n`n### Checklist`n- [x] Code committed`n- [x] Branch pushed`n- [ ] Review required" `
    --base main `
    --head $branchName
```

#### Option B: Generate PR URL (Semi-Automated)

```powershell
# Get repository info
$repoUrl = git config --get remote.origin.url
$repoUrl = $repoUrl -replace '\.git$', ''
$repoUrl = $repoUrl -replace 'git@github.com:', 'https://github.com/'

# Generate PR creation URL
$prUrl = "$repoUrl/pull/new/$branchName"
Write-Host "Open this URL to create PR: $prUrl"
Start-Process $prUrl
```

## Complete PowerShell Script

Save this as `auto-pr.ps1`:

```powershell
# Auto Create Pull Request Script
param(
    [string]$CommitMessage = "",
    [string]$PRTitle = "",
    [string]$PRBody = "",
    [switch]$SkipReview
)

function Write-Status {
    param([string]$Message, [string]$Color = "Cyan")
    Write-Host "[$([DateTime]::Now.ToString('HH:mm:ss'))] $Message" -ForegroundColor $Color
}

function Get-RepoInfo {
    $repoUrl = git config --get remote.origin.url
    $repoUrl = $repoUrl -replace '\.git$', ''
    $repoUrl = $repoUrl -replace 'git@github.com:', 'https://github.com/'
    return $repoUrl
}

function Get-BranchName {
    $currentBranch = git branch --show-current
    return $currentBranch
}

function Create-FeatureBranch {
    $timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $branchName = "feature/auto-$timestamp"
    
    Write-Status "Creating feature branch: $branchName" "Yellow"
    git checkout -b $branchName
    
    if ($LASTEXITCODE -ne 0) {
        Write-Status "Failed to create branch" "Red"
        exit 1
    }
    
    return $branchName
}

function Stage-Changes {
    Write-Status "Staging all changes..." "Yellow"
    git add .
    
    $status = git status --short
    if ([string]::IsNullOrWhiteSpace($status)) {
        Write-Status "No changes to commit" "Red"
        exit 1
    }
    
    Write-Status "Changes staged successfully" "Green"
}

function Commit-Changes {
    param([string]$Message)
    
    if ([string]::IsNullOrWhiteSpace($Message)) {
        # Auto-generate commit message
        $newFiles = @(git diff --cached --name-only --diff-filter=A)
        $modifiedFiles = @(git diff --cached --name-only --diff-filter=M)
        $deletedFiles = @(git diff --cached --name-only --diff-filter=D)
        
        if ($newFiles.Count -gt 0) {
            $Message = "feat: Add $($newFiles.Count) new file(s)"
        } elseif ($modifiedFiles.Count -gt 0) {
            $Message = "fix: Update $($modifiedFiles.Count) file(s)"
        } elseif ($deletedFiles.Count -gt 0) {
            $Message = "chore: Remove $($deletedFiles.Count) file(s)"
        } else {
            $Message = "chore: Update repository"
        }
    }
    
    Write-Status "Committing with message: $Message" "Yellow"
    git commit -m $Message
    
    if ($LASTEXITCODE -ne 0) {
        Write-Status "Commit failed" "Red"
        exit 1
    }
    
    Write-Status "Commit successful" "Green"
    return $Message
}

function Push-Branch {
    param([string]$BranchName)
    
    Write-Status "Pushing branch to remote..." "Yellow"
    git push origin $BranchName
    
    if ($LASTEXITCODE -ne 0) {
        Write-Status "Push failed" "Red"
        exit 1
    }
    
    Write-Status "Push successful" "Green"
}

function Create-PullRequest {
    param(
        [string]$Title,
        [string]$Body,
        [string]$BranchName
    )
    
    # Check if GitHub CLI is available
    $ghInstalled = Get-Command gh -ErrorAction SilentlyContinue
    
    if ($ghInstalled) {
        Write-Status "Creating PR using GitHub CLI..." "Yellow"
        
        if ([string]::IsNullOrWhiteSpace($Body)) {
            $Body = @"
## Auto-generated Pull Request

This PR was created automatically using the auto-create-pull-request workflow.

### Changes
- Auto-committed changes from local repository

### Files Changed
$(git diff --name-only origin/main..$BranchName | ForEach-Object { "- $_" })

### Checklist
- [x] Code committed
- [x] Branch pushed
- [ ] Review required
- [ ] Tests passing
"@
        }
        
        gh pr create --title $Title --body $Body --base main --head $BranchName
        
        if ($LASTEXITCODE -eq 0) {
            Write-Status "Pull request created successfully!" "Green"
            return $true
        } else {
            Write-Status "Failed to create PR via CLI" "Red"
            return $false
        }
    } else {
        Write-Status "GitHub CLI not installed, generating PR URL..." "Yellow"
        $repoUrl = Get-RepoInfo
        $prUrl = "$repoUrl/pull/new/$BranchName"
        
        Write-Host "`n" -NoNewline
        Write-Status "Open this URL to create PR:" "Cyan"
        Write-Host $prUrl -ForegroundColor White
        Write-Host "`n" -NoNewline
        
        # Try to open in browser
        try {
            Start-Process $prUrl
            Write-Status "Opened browser to create PR" "Green"
        } catch {
            Write-Status "Please manually open the URL above" "Yellow"
        }
        
        return $false
    }
}

# Main execution
try {
    Write-Host "`n"
    Write-Status "=== Auto Create Pull Request Workflow ===" "Cyan"
    Write-Host "`n"
    
    # Step 1: Check current branch
    $currentBranch = Get-BranchName
    Write-Status "Current branch: $currentBranch" "Cyan"
    
    # Step 2: Create feature branch if on main
    if ($currentBranch -eq "main" -or $currentBranch -eq "master") {
        $branchName = Create-FeatureBranch
    } else {
        $branchName = $currentBranch
        Write-Status "Using existing branch: $branchName" "Cyan"
    }
    
    # Step 3: Stage changes
    Stage-Changes
    
    # Step 4: Commit changes
    $commitMsg = Commit-Changes -Message $CommitMessage
    
    # Step 5: Push branch
    Push-Branch -BranchName $branchName
    
    # Step 6: Create PR
    if ([string]::IsNullOrWhiteSpace($PRTitle)) {
        $PRTitle = $commitMsg
    }
    
    $prCreated = Create-PullRequest -Title $PRTitle -Body $PRBody -BranchName $branchName
    
    Write-Host "`n"
    Write-Status "=== Workflow Complete ===" "Green"
    Write-Host "`n"
    
    if ($prCreated) {
        Write-Status "✓ Pull request created and ready for review!" "Green"
    } else {
        Write-Status "✓ Branch pushed. Complete PR creation in browser." "Yellow"
    }
    
} catch {
    Write-Status "Error: $_" "Red"
    exit 1
}
```

## Usage

### Basic Usage (Auto-detect everything)

```powershell
.\auto-pr.ps1
```

### With Custom Commit Message

```powershell
.\auto-pr.ps1 -CommitMessage "feat: Add new feature"
```

### With Custom PR Title and Body

```powershell
.\auto-pr.ps1 `
    -CommitMessage "feat: Add authentication" `
    -PRTitle "Add user authentication system" `
    -PRBody "Implements JWT-based authentication with refresh tokens"
```

### Skip Review (Auto-merge if possible)

```powershell
.\auto-pr.ps1 -SkipReview
```

## Node.js Alternative Script

Save as `auto-pr.js`:

```javascript
const { execSync } = require('child_process');
const readline = require('readline');

function exec(command) {
    try {
        return execSync(command, { encoding: 'utf8' }).trim();
    } catch (error) {
        console.error(`Error executing: ${command}`);
        throw error;
    }
}

function getCurrentBranch() {
    return exec('git branch --show-current');
}

function createFeatureBranch() {
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, -5);
    const branchName = `feature/auto-${timestamp}`;
    console.log(`Creating branch: ${branchName}`);
    exec(`git checkout -b ${branchName}`);
    return branchName;
}

function stageAndCommit(message) {
    console.log('Staging changes...');
    exec('git add .');
    
    if (!message) {
        const status = exec('git status --short');
        const lines = status.split('\n');
        const added = lines.filter(l => l.startsWith('A')).length;
        const modified = lines.filter(l => l.startsWith('M')).length;
        
        if (added > 0) {
            message = `feat: Add ${added} new file(s)`;
        } else if (modified > 0) {
            message = `fix: Update ${modified} file(s)`;
        } else {
            message = 'chore: Update repository';
        }
    }
    
    console.log(`Committing: ${message}`);
    exec(`git commit -m "${message}"`);
    return message;
}

function pushBranch(branchName) {
    console.log('Pushing to remote...');
    exec(`git push origin ${branchName}`);
}

function createPR(title, branchName) {
    try {
        console.log('Creating pull request...');
        exec(`gh pr create --title "${title}" --body "Auto-generated PR" --base main --head ${branchName}`);
        console.log('✓ Pull request created!');
    } catch (error) {
        const repoUrl = exec('git config --get remote.origin.url')
            .replace('.git', '')
            .replace('git@github.com:', 'https://github.com/');
        const prUrl = `${repoUrl}/pull/new/${branchName}`;
        console.log(`\nOpen this URL to create PR:\n${prUrl}`);
    }
}

// Main execution
(async () => {
    try {
        console.log('\n=== Auto Create Pull Request ===\n');
        
        const currentBranch = getCurrentBranch();
        let branchName = currentBranch;
        
        if (currentBranch === 'main' || currentBranch === 'master') {
            branchName = createFeatureBranch();
        }
        
        const commitMessage = stageAndCommit();
        pushBranch(branchName);
        createPR(commitMessage, branchName);
        
        console.log('\n✓ Workflow complete!\n');
    } catch (error) {
        console.error('Error:', error.message);
        process.exit(1);
    }
})();
```

## Features

✅ **Automatic branch creation** - Creates feature branch if on main  
✅ **Smart commit messages** - Auto-generates conventional commit messages  
✅ **Auto-staging** - Stages all changes automatically  
✅ **Push automation** - Pushes branch to remote  
✅ **PR creation** - Creates PR via GitHub CLI or browser  
✅ **Error handling** - Graceful fallbacks and error messages  
✅ **Customizable** - Override any auto-generated values  

## Configuration

### Install GitHub CLI (for full automation)

**Windows:**
```powershell
winget install --id GitHub.cli
```

**Or download from:** https://cli.github.com/

### Authenticate GitHub CLI

```powershell
gh auth login
```

## Troubleshooting

### GitHub CLI not found
- Install GitHub CLI using the instructions above
- Restart your terminal after installation

### Authentication errors
- Run `gh auth login` to authenticate
- Ensure you have push access to the repository

### Branch already exists
- The script will use the existing branch
- Or manually delete the branch: `git branch -D branch-name`

## Best Practices

1. **Review before running** - Check your changes with `git status`
2. **Use custom messages** - Provide meaningful commit messages
3. **Test locally** - Ensure code works before auto-creating PR
4. **Set up CI/CD** - Automated tests will catch issues
5. **Configure branch protection** - Require reviews even for auto-PRs

## Integration with IDE

Add to your IDE tasks or shortcuts:

**VS Code tasks.json:**
```json
{
    "label": "Auto Create PR",
    "type": "shell",
    "command": "pwsh -File .windsurf/workflows/scripts/auto-pr.ps1",
    "problemMatcher": []
}
```
