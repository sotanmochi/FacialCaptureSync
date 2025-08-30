# FacialCaptureSync

FacialCaptureSync is a library for synchronizing facial capture data into Unity in real time.  
The following applications are supported as facial capture sources.

- iFacialMocap ([日本語](https://www.ifacialmocap.com/home/japanese/), [English](https://www.ifacialmocap.com/))
    - iFacialMocap (iOS app) ([jp](https://apps.apple.com/jp/app/ifacialmocap/id1489470545), [us](https://apps.apple.com/us/app/ifacialmocap/id1489470545)) 
    - iFacialMocap to Software (PC app) ([日本語](https://www.ifacialmocap.com/download/japanese/), [English](https://www.ifacialmocap.com/download/))

- Facemotion3d ([日本語](https://www.ifacialmocap.com/upgrade/%E6%97%A5%E6%9C%AC%E8%AA%9E/), [English](https://www.ifacialmocap.com/upgrade/))
    - Facemotion3d (iOS app) ([jp](https://apps.apple.com/jp/app/facemotion3d/id1507538005), [us](https://apps.apple.com/us/app/facemotion3d/id1507538005))

## Tested Environment
- Unity 2021.3.24f1

## Installation
You can install via Package Manager in UnityEditor.

1. Open the Package Manager window
2. Click the + button and select "Add package from git URL"
3. Enter: `https://github.com/sotanmochi/FacialCaptureSync.git?path=src/FacialCaptureSync/Packages/FacialCaptureSync#1.1.0`

You can also install via editing Packages/manifest.json directly.
```
// Packages/manifest.json
{
  "dependencies": {
    ...
    "jp.sotanmochi.facialcapturesync": "https://github.com/sotanmochi/FacialCaptureSync.git?path=src/FacialCaptureSync/Packages/FacialCaptureSync#1.1.0",
    ...
  }
}
```

## License
This library is licensed under the MIT License.
