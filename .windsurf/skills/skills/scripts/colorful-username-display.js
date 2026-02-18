const os = require('os');

// ANSI color codes
const colors = {
    reset: '\x1b[0m',
    bright: '\x1b[1m',
    
    // Foreground colors
    red: '\x1b[31m',
    green: '\x1b[32m',
    yellow: '\x1b[33m',
    blue: '\x1b[34m',
    magenta: '\x1b[35m',
    cyan: '\x1b[36m',
    white: '\x1b[37m',
    
    // Background colors
    bgRed: '\x1b[41m',
    bgGreen: '\x1b[42m',
    bgYellow: '\x1b[43m',
    bgBlue: '\x1b[44m',
    bgMagenta: '\x1b[45m',
    bgCyan: '\x1b[46m',
    bgWhite: '\x1b[47m'
};

function displayColorfulBubbles() {
    console.log(`\n${colors.bright}${colors.cyan}╔════════════════════════════════════════╗${colors.reset}`);
    console.log(`${colors.bright}${colors.cyan}║${colors.reset}     ${colors.magenta}✨ Colorful Username Display ✨${colors.reset}     ${colors.bright}${colors.cyan}║${colors.reset}`);
    console.log(`${colors.bright}${colors.cyan}╚════════════════════════════════════════╝${colors.reset}\n`);
}

function displayUsername() {
    const username = os.userInfo().username;
    
    console.log(`${colors.bright}${colors.yellow}👤 Username:${colors.reset} ${colors.bright}${colors.green}${username}${colors.reset}\n`);
}

function displayColorfulBubblesArt() {
    console.log(`${colors.bright}${colors.white}Multi-Color Bubbles:${colors.reset}\n`);
    
    // Row 1
    console.log(`  ${colors.bgRed}${colors.white}  ●  ${colors.reset}  ${colors.bgGreen}${colors.white}  ●  ${colors.reset}  ${colors.bgYellow}${colors.white}  ●  ${colors.reset}  ${colors.bgBlue}${colors.white}  ●  ${colors.reset}`);
    
    // Row 2
    console.log(`  ${colors.bgMagenta}${colors.white}  ●  ${colors.reset}  ${colors.bgCyan}${colors.white}  ●  ${colors.reset}  ${colors.bgRed}${colors.white}  ●  ${colors.reset}  ${colors.bgGreen}${colors.white}  ●  ${colors.reset}`);
    
    // Row 3
    console.log(`  ${colors.bgYellow}${colors.white}  ●  ${colors.reset}  ${colors.bgBlue}${colors.white}  ●  ${colors.reset}  ${colors.bgMagenta}${colors.white}  ●  ${colors.reset}  ${colors.bgCyan}${colors.white}  ●  ${colors.reset}\n`);
}

function displayRainbowPalette() {
    console.log(`${colors.bright}${colors.white}Rainbow Color Palette:${colors.reset}\n`);
    console.log(`${colors.red}██${colors.reset} ${colors.green}██${colors.reset} ${colors.yellow}██${colors.reset} ${colors.blue}██${colors.reset} ${colors.magenta}██${colors.reset} ${colors.cyan}██${colors.reset} ${colors.white}██${colors.reset}\n`);
}

function displaySystemInfo() {
    console.log(`${colors.bright}${colors.cyan}System Information:${colors.reset}`);
    console.log(`${colors.green}Platform:${colors.reset}  ${colors.white}${os.platform()}${colors.reset}`);
    console.log(`${colors.green}Hostname:${colors.reset}  ${colors.white}${os.hostname()}${colors.reset}`);
    console.log(`${colors.green}OS Type:${colors.reset}   ${colors.white}${os.type()}${colors.reset}\n`);
}

function displaySuccessMessage() {
    console.log(`${colors.bright}${colors.green}╔════════════════════════════════════════╗${colors.reset}`);
    console.log(`${colors.bright}${colors.green}║${colors.reset}  ${colors.magenta}✓${colors.reset} ${colors.white}Skill Executed Successfully!${colors.reset}      ${colors.bright}${colors.green}║${colors.reset}`);
    console.log(`${colors.bright}${colors.green}╚════════════════════════════════════════╝${colors.reset}\n`);
}

// Main execution
displayColorfulBubbles();
displayUsername();
displayColorfulBubblesArt();
displayRainbowPalette();
displaySystemInfo();
displaySuccessMessage();