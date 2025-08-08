# Stems Visualizer

A Unity-based audio visualization application for analyzing and visualizing music stems with real-time audio-reactive animations.

## Overview

This project creates dynamic visual representations of music by separating audio tracks into individual stems (vocals, drums, bass, other) and generating synchronized visualizations. The application connects to an external audio analysis server to process music files and extract audio segments, beats, tempo, and spectral data.

## Features

- Audio stem separation and analysis
- Real-time audio-reactive visualizations
- Toggling of individual or multiple stems
- Beat detection and tempo analysis
- Song structure segmentation (intro, verse, chorus, bridge, etc.)
- Video recording of running visualization
- Interactive file browser for audio selection
- HDRP rendering pipeline support

## Dependencies

- Unity 2022.3 or later
- HDRP rendering pipelines
- External audio analysis server running on localhost:5000
- Various Unity packages for audio processing and UI
