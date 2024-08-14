# IMYSHook

## [中文](README_TC.md)

Iris Mysteria mod for DMM Game Player version

## Feature

1. Game speed
2. In-game Screenshot
3. FPS setting
4. [Translation](Translation.md) 

## Requirement

1. Windows 10 or newer
2. Iris Mysteria DMM Game Player version

## Installation

Download and extract [Release](https://github.com/IrisMystery/IMYSHook/releases) zip to your Iris Mysteria install
location `C:\Users\<username>\imys_r_exe`

## Config

You can edit config.json(`./BepInEx/plugins/config.json`) if you don't like default settings.

| Name      | Default Value | Description                                        |
|-----------|---------------|----------------------------------------------------|
| speed     | 0.5           | Increase/Decrease game speed each step (per click) | 
| fps       | 60            | Override FPS setting                               |
| translate | true          | Enable/Disable translation feature                 |

## Key binding

| Key | Type        | Description                                                                   |
|-----|-------------|-------------------------------------------------------------------------------|
| F5  | Freeze      | Freeze game, mean set game speed to 0x                                        |
| F6  | Reset       | Reset game speed to 1x/normal                                                 | 
| F7  | Decrease    | Decrease game speed (2-0.5 etc), depends on your `speed` config               | 
| F8  | Increase    | Increase game speed (1+0.5 etc), depends on your `speed` config               |
| F10 | Translation | Clear translation cache                                                       |
| F11 | Translation | Enable/Disable translation feature                                            |
| F12 | Screenshot  | Screenshot current frame and save to Pictures(`C:\Users\<username>\Pictures`) |

## Contributing

You're free to contribute to IMYSHook as long as the features are useful, such as battle stats log, full AP/BP alert or
something else, except modifying battle data.

## Disclaimer

Using IMYSHook violates Iris Mysteria and DMM's terms of service.

I will NOT be held responsible for any bans!
