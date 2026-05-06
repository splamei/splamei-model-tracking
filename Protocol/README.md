
Below is an example of the data that bridge apps need to send to work with this app via UDP port 58080:

```json
{
  "version": 1,
  "type": "Full",
  "runtimeId": 780779417,
  "friendlyName": "Example tracker - Bridge app",
  "identifier": "EXAMPLE_TRACKER_BRIDGE_APP",
  "frame": 8844,
  "timestampL": 41896,
  "timestampG": 63913509464309,
  "sendSpeed": 30,
  "head": {
    "x": 0,
    "y": 1.7,
    "z": 0
  },
  "neck": {
    "x": 0,
    "y": 1.4,
    "z": 0
  },
  "spineBase": {
    "x": 0,
    "y": 0,
    "z": 0
  },
  "shoulderLeft": {
    "x": -0.2,
    "y": 1.4,
    "z": 0
  },
  "shoulderRight": {
    "x": 0.2,
    "y": 1.4,
    "z": 0
  },
  "elbowLeft": {
    "x": -0.6,
    "y": 1.4,
    "z": 0
  },
  "elbowRight": {
    "x": 0.6,
    "y": 1.4,
    "z": 0
  },
  "wristLeft": {
    "x": -1,
    "y": 1.4,
    "z": 0
  },
  "wristRight": {
    "x": 1,
    "y": 1.4,
    "z": 0
  },
  "handLeft": {
    "x": -1,
    "y": 1.4,
    "z": 0
  },
  "handRight": {
    "x": 1,
    "y": 1.4,
    "z": 0
  },
  "hipLeft": {
    "x": -0.2,
    "y": 0.9,
    "z": 0
  },
  "hipRight": {
    "x": 0.2,
    "y": 0.9,
    "z": 0
  },
  "kneeLeft": {
    "x": -0.2,
    "y": 0.5,
    "z": 0
  },
  "kneeRight": {
    "x": 0.2,
    "y": 0.5,
    "z": 0
  },
  "ankleLeft": {
    "x": -0.2,
    "y": 0.1,
    "z": 0
  },
  "ankleRight": {
    "x": 0.2,
    "y": 0.1,
    "z": 0
  },
  "footLeft": {
    "x": -0.2,
    "y": 0.1,
    "z": 0
  },
  "footRight": {
    "x": 0.2,
    "y": 0.1,
    "z": 0
  }
}
```

And here's what everything means:
| Key | Meaning | Example |
|--|--|--|
| version | The version of the protocol being used | `1.0` |
| type | The type of tracking (currently only 'Full') | `Full` |
| runtimeId | A random int generated at startup to identify different bridge apps | `780779417` |
| friendlyName | The public name for your bridge app or tracker | `Example tracker - Bridge app` |
| identifier | The back-end name for your app. You should use upper snake case | `EXAMPLE_TRACKER_BRIDGE_APP` |
| frame | The frames that passed during tracking. This may mean the times the tracker has sent it's data or frames passed in your bridge app | `8844` |
| timestampL | The milliseconds passed since the bridge app started tracking | `41896` |
| timestampG | The total ticks globally that elapsed in UTC time | `63913509464309` |
| sendSpeed | The total amount of updates you're providing per second (Hz) | `30` |

The rest of the data corresponds to the armature of the person being tracked. The key name is the ID name of the joint and the value contains the X, Y and Z values from the tracker. You should try and keep the values small and relative to game space however, the receiving app should clamp and adjust these values to these needs and not rely on the bridge app.
