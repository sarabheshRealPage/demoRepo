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
            $changedFiles = git diff --name-only origin/main..$BranchName | ForEach-Object { "- $_" }
            $Body = @"
## Auto-generated Pull Request

This PR was created automatically using the auto-create-pull-request workflow.

### Changes
- Auto-committed changes from local repository

### Files Changed
$($changedFiles -join "`n")

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
