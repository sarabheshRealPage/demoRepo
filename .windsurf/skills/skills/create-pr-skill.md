---
name: create-pull-request
description: Automatically creates a pull request with current changes
phase: "create-pr"
---

# Create Pull Request Skill

This skill automates the process of creating a pull request from your current branch.

## Workflow

1. Check current branch and git status
2. Stage all changes
3. Commit changes with a descriptive message
4. Push branch to remote
5. Create pull request using GitHub CLI (gh) or provide instructions

## Steps

### 1. Verify Git Status
- Check if there are uncommitted changes
- Identify current branch name
- Ensure branch is not main/master

### 2. Stage and Commit
- Stage all changes: `git add .`
- Commit with message: `git commit -m "feat: [description]"`

### 3. Push to Remote
- Push current branch: `git push -u origin [branch-name]`

### 4. Create Pull Request
- Using GitHub CLI: `gh pr create --title "[Title]" --body "[Description]"`
- Or provide GitHub URL to create PR manually

## Requirements

- Git installed and configured
- GitHub CLI (gh) installed (optional but recommended)
- Current branch should not be main/master
- Remote repository configured

## Usage

Type `create-pr` in the chat to trigger this skill.
