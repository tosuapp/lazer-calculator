const platform = process.platform;
switch (platform) {
  case "win32":
    module.exports = require('@tosuapp/lazer-calculator-win32-x64');
    break;

  case "linux":
    module.exports = require('@tosuapp/lazer-calculator-linux-x64');
    break;

  default:
    throw new Error(`Unsupported platform: ${platform}`);
}
