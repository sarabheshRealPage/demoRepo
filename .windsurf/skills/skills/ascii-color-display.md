---
name: ascii-color-display
description: Display colorful ASCII art output with different colors
phase: "ascii-display"
---

# ASCII Color Display Skill

Use this skill when the user wants to display ASCII art with different colors.

## Workflow

1. Generate ASCII art text
2. Apply ANSI color codes for terminal output
3. Display the colorful ASCII art

## Implementation

### ANSI Color Codes Reference

- **Reset**: `\033[0m`
- **Red**: `\033[91m`
- **Green**: `\033[92m`
- **Yellow**: `\033[93m`
- **Blue**: `\033[94m`
- **Magenta**: `\033[95m`
- **Cyan**: `\033[96m`
- **White**: `\033[97m`
- **Bold**: `\033[1m`

### Example ASCII Art Template

```
\033[91m  ██████╗ ███████╗██╗   ██╗
\033[92m  ██╔══██╗██╔════╝██║   ██║
\033[93m  ██║  ██║█████╗  ██║   ██║
\033[94m  ██║  ██║██╔══╝  ╚██╗ ██╔╝
\033[95m  ██████╔╝███████╗ ╚████╔╝ 
\033[96m  ╚═════╝ ╚══════╝  ╚═══╝  
\033[0m
```

## Usage

Run the script to display colorful ASCII art:

```bash
node ./scripts/ascii-color-display.js
```

Or use PowerShell:

```powershell
.\scripts\ascii-color-display.ps1
```

## Response Format

When this skill is triggered, display:
1. A colorful ASCII banner
2. System information in colored sections
3. A success message with color highlighting
