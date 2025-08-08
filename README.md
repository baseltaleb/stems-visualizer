# Stems Visualizer

A Unity-based audio visualization application for analyzing and visualizing music segments and stems.

## Overview

The idea of this project is to create a visualizer that dynamically reacts to music segments (intro, verse, chorus, bridge, etc.), by changing scene settings (lighting, colors, particles etc.). You can also visualize individual song stems (vocals, drums, bass, other), or a combination of them. The application connects to an external audio analysis server to process music files and extract audio segments, beats, tempo, and spectral data.

| Verse | Bridge | Solo |
| ----- | ------ | ---- |
| <img alt="image" src="https://github.com/user-attachments/assets/e79ac2fd-483e-4e81-9923-59cb513380f1" /> | <img alt="image" src="https://github.com/user-attachments/assets/fb61fa13-975d-4eed-a89f-a9b030fcbfbd" /> | <img alt="image" src="https://github.com/user-attachments/assets/7f1c3afc-0260-4c5c-8d5d-0defd74eb5c9" /> |

## Features
- Dynamically reacts to music segments (intro, verse, chorus, bridge, etc.)
- Audio stem separation and analysis
- Toggling of individual or multiple stems
- Beat detection and tempo analysis
- Video recording of running visualization
- HDRP rendering pipeline support

## Dependencies

- Unity 6 (6000.0.32f1) or later
- HDRP rendering pipelines
- External audio analysis server running on localhost:5000
- Unity VFX Graph
- Various Unity packages for audio processing and UI
