---
description: Raise a pull request for code changes
---

# Raise Pull Request Workflow

Follow these steps to create and submit a pull request for your code changes.

## Prerequisites

- Ensure you have committed your changes locally
- Verify you're on the correct feature branch
- Code has been tested and reviewed locally

## Steps

### 1. Check Current Branch Status

```powershell
git status
git branch
```

Verify you're on your feature branch and all changes are committed.

### 2. Update Your Branch with Latest Changes

```powershell
git fetch origin
git rebase origin/main
```

Or if you prefer merge:

```powershell
git pull origin main
```

Resolve any conflicts if they arise.

### 3. Run Local Tests

Ensure all tests pass before pushing:

```powershell
# Run your test suite
npm test
# or
dotnet test
```

### 4. Push Your Branch to Remote

```powershell
git push origin <your-branch-name>
```

If you've rebased, you may need to force push:

```powershell
git push origin <your-branch-name> --force-with-lease
```

### 5. Create Pull Request

#### Option A: Using GitHub CLI

```powershell
gh pr create --title "Your PR Title" --body "Description of changes" --base main
```

#### Option B: Using Web Interface

1. Navigate to your repository on GitHub
2. Click "Pull requests" tab
3. Click "New pull request"
4. Select your branch as the compare branch
5. Fill in the PR template:
   - **Title**: Clear, concise description
   - **Description**: What changes were made and why
   - **Related Issues**: Link any related issues (#issue-number)
   - **Testing**: Describe how you tested the changes
   - **Screenshots**: Add if UI changes were made

### 6. PR Checklist

Before submitting, ensure:

- [ ] Code follows project coding standards
- [ ] All tests pass
- [ ] Documentation is updated
- [ ] Commit messages are clear and descriptive
- [ ] No merge conflicts
- [ ] Code has been self-reviewed
- [ ] No sensitive data (API keys, passwords) in code

### 7. Request Reviews

```powershell
gh pr review <pr-number> --request-reviewer @username
```

Or assign reviewers through the GitHub web interface.

### 8. Address Review Comments

When reviewers provide feedback:

1. Make necessary changes locally
2. Commit the changes:
   ```powershell
   git add .
   git commit -m "Address review comments: <description>"
   ```
3. Push updates:
   ```powershell
   git push origin <your-branch-name>
   ```

### 9. Merge Pull Request

Once approved:

#### Option A: Using GitHub CLI

```powershell
gh pr merge <pr-number> --squash
# or --merge or --rebase depending on team preference
```

#### Option B: Using Web Interface

1. Click "Merge pull request" button
2. Choose merge strategy (squash, merge, or rebase)
3. Confirm merge
4. Delete branch after merge (optional but recommended)

### 10. Clean Up Local Branch

```powershell
git checkout main
git pull origin main
git branch -d <your-branch-name>
```

## PR Title Conventions

Use conventional commit format:

- `feat: Add new feature`
- `fix: Fix bug in component`
- `docs: Update documentation`
- `refactor: Refactor code structure`
- `test: Add unit tests`
- `chore: Update dependencies`

## PR Description Template

```markdown
## Description
Brief description of what this PR does.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Related Issues
Fixes #(issue number)

## Changes Made
- Change 1
- Change 2
- Change 3

## Testing
Describe how you tested these changes.

## Screenshots (if applicable)
Add screenshots here.

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex code
- [ ] Documentation updated
- [ ] No new warnings generated
- [ ] Tests added/updated
- [ ] All tests passing
```

## Troubleshooting

### Merge Conflicts

```powershell
git fetch origin
git rebase origin/main
# Resolve conflicts in your editor
git add .
git rebase --continue
git push origin <your-branch-name> --force-with-lease
```

### Failed CI/CD Checks

1. Check the CI/CD logs in the PR
2. Fix issues locally
3. Commit and push fixes
4. CI/CD will automatically re-run

### Need to Update PR After Review

```powershell
# Make changes
git add .
git commit -m "Update based on review feedback"
git push origin <your-branch-name>
```

## Best Practices

1. **Keep PRs Small**: Easier to review and merge
2. **Write Clear Descriptions**: Help reviewers understand changes
3. **Link Issues**: Connect PR to related issues
4. **Respond Promptly**: Address review comments quickly
5. **Test Thoroughly**: Ensure changes work as expected
6. **Update Documentation**: Keep docs in sync with code changes
7. **Use Draft PRs**: For work-in-progress that needs early feedback

## GitHub CLI Quick Reference

```powershell
# Create PR
gh pr create

# List PRs
gh pr list

# View PR details
gh pr view <pr-number>

# Check PR status
gh pr status

# Merge PR
gh pr merge <pr-number>

# Close PR
gh pr close <pr-number>
```
