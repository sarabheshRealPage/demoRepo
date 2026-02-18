const os = require('os');

const colors = {
    reset: '\x1b[0m',
    bright: '\x1b[1m',
    red: '\x1b[91m',
    green: '\x1b[92m',
    yellow: '\x1b[93m',
    blue: '\x1b[94m',
    magenta: '\x1b[95m',
    cyan: '\x1b[96m',
    white: '\x1b[97m'
};

function displayColorfulBanner() {
    console.log(`
${colors.red}  ███████╗██╗  ██╗██╗██╗     ██╗     
${colors.yellow}  ██╔════╝██║ ██╔╝██║██║     ██║     
${colors.green}  ███████╗█████╔╝ ██║██║     ██║     
${colors.cyan}  ╚════██║██╔═██╗ ██║██║     ██║     
${colors.blue}  ███████║██║  ██╗██║███████╗███████╗
${colors.magenta}  ╚══════╝╚═╝  ╚═╝╚═╝╚══════╝╚══════╝
${colors.reset}
    `);
}

function displaySystemInfo() {
    console.log(`${colors.bright}${colors.cyan}╔════════════════════════════════════════╗${colors.reset}`);
    console.log(`${colors.bright}${colors.cyan}║${colors.reset}     ${colors.yellow}SYSTEM INFORMATION${colors.reset}              ${colors.bright}${colors.cyan}║${colors.reset}`);
    console.log(`${colors.bright}${colors.cyan}╚════════════════════════════════════════╝${colors.reset}\n`);
    
    console.log(`${colors.green}Platform:${colors.reset}     ${colors.white}${os.platform()}${colors.reset}`);
    console.log(`${colors.green}Architecture:${colors.reset} ${colors.white}${os.arch()}${colors.reset}`);
    console.log(`${colors.green}Hostname:${colors.reset}     ${colors.white}${os.hostname()}${colors.reset}`);
    console.log(`${colors.green}CPUs:${colors.reset}         ${colors.white}${os.cpus().length} cores${colors.reset}`);
    console.log(`${colors.green}Total Memory:${colors.reset} ${colors.white}${(os.totalmem() / (1024 ** 3)).toFixed(2)} GB${colors.reset}`);
    console.log(`${colors.green}Free Memory:${colors.reset}  ${colors.white}${(os.freemem() / (1024 ** 3)).toFixed(2)} GB${colors.reset}`);
    console.log(`${colors.green}Uptime:${colors.reset}       ${colors.white}${(os.uptime() / 3600).toFixed(2)} hours${colors.reset}\n`);
}

function displaySuccessMessage() {
    console.log(`${colors.bright}${colors.green}╔════════════════════════════════════════╗${colors.reset}`);
    console.log(`${colors.bright}${colors.green}║${colors.reset}  ${colors.magenta}✓${colors.reset} ${colors.white}Skill Executed Successfully!${colors.reset}      ${colors.bright}${colors.green}║${colors.reset}`);
    console.log(`${colors.bright}${colors.green}╚════════════════════════════════════════╝${colors.reset}\n`);
}

function displayColorPalette() {
    console.log(`${colors.bright}${colors.white}Color Palette Demo:${colors.reset}\n`);
    console.log(`${colors.red}■ Red${colors.reset}     ${colors.green}■ Green${colors.reset}   ${colors.yellow}■ Yellow${colors.reset}`);
    console.log(`${colors.blue}■ Blue${colors.reset}    ${colors.magenta}■ Magenta${colors.reset} ${colors.cyan}■ Cyan${colors.reset}\n`);
}

displayColorfulBanner();
displaySystemInfo();
displayColorPalette();
displaySuccessMessage();
