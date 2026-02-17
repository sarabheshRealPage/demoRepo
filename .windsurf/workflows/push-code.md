---
description: Push code changes to the repository
---

# Push Code Workflow

This workflow guides you through pushing your code changes to the remote repository.

## Steps

1. **Check the status of your changes**
   ```bash
   git status
   ```

2. **Stage all changes**
   ```bash
   git add .
   ```
   Or stage specific files:
   ```bash
   git add <file-path>
   ```

3. **Commit your changes with a descriptive message**
   ```bash
   git commit -m "Your commit message here"
   ```

4. **Pull the latest changes from remote (to avoid conflicts)**
   ```bash
   git pull origin main
   ```
   Note: Replace `main` with your branch name if different (e.g., `master`, `develop`)

5. **Push your changes to the remote repository**
   ```bash
   git push origin main
   ```
   Note: Replace `main` with your branch name if different

## Alternative: Push to a new branch

If you want to push to a new branch:

1. **Create and switch to a new branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Push the new branch to remote**
   ```bash
   git push -u origin feature/your-feature-name
   ```

## Troubleshooting

- If you encounter merge conflicts during pull, resolve them manually and then commit
- If push is rejected, you may need to pull first or force push (use with caution)
- Always ensure you're on the correct branch before pushing
