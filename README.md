# SyDH: An Audiovisual Speech Corpus of Audio-Driven Digital Humans

<div align="center">
  
[![CC BY-NC 4.0][cc-by-nc-shield]][cc-by-nc] [![Dataset](https://img.shields.io/badge/Link-Dataset-purple)](https://tubcloud.tu-berlin.de/s/4NikeRcsfTC8XXW/download) [![Unity](https://img.shields.io/badge/Unity-2020.3+-black.svg)](https://unity.com/releases/editor/whats-new/2020.3.48f1#installs)
  
<img alt="Header with talkin heads" src="Readme_Figs/header14x4.gif"> </img>

</div>

A large-scale dataset with 27,200 video clips of audio-driven digital human talking heads for speech perception research

## 🌟 About SyDH

**SyDH** (Synthetic Digital Humans) is a comprehensive audiovisual corpus comprising **27,200 video clips** (22.6+ hours) of audio-driven digital human talking heads, designed to study how synthetic faces influence audiovisual speech perception. SyDH uses production-ready 3D digital humans with full parametric control and compability with existing pipelines, enabling deployment to different platforms and fine-grained editability over all aspects of the videos. A subset with varying lighting and animation post-processing parameters (expressiveness, symmetry, smoothing) facilitates investigation of how different variables influence perception and effectiveness of talking digital humans.

The dataset addresses the growing need for controlled, reproducible research on how virtual humans support and shape communication across XR, gaming, education, healthcare, and customer service applications.



### 📊 Dataset Overview
- **Total Clips:** 27,200
- **Duration:** 22.6+ hours
- **Digital Humans:** 12 unique 3D characters
- **Speakers:** 12 (GRID corpus, gender-matched, 6 female, 6 male)
- **Resolution:** 1920×1080 px at 60 FPS
- **Parameters Controlled:** Lighting, expressiveness, symmetry, temporal smoothing


### 📢 Dataset Release
You can download the full dataset here: 
- [Direct Download](https://tubcloud.tu-berlin.de/s/4NikeRcsfTC8XXW/download)
- [(Privately shared) Kaggle Repository](https://kaggle.com/datasets/cdcc7c9b236d8cac723625e5a161ccf450c89621c614a73783c5d025806476b9)

---

## Unity Project 

All video clips of the SyDH corpus can be reproduced and optionally modified using this Unity project.

### Prerequisites
- **Unity**: The project was developed using [Unity 2020.3.48f1](https://unity.com/releases/editor/whats-new/2020.3.48f1#installs). Make sure you to use this or a compatible version.
- **Dependencies**:  
  - The **CC/iC Unity Tools package** is required for importing the characters. [Version 1.6.3 for HDRP](https://github.com/soupday/cc_unity_tools_HDRP/releases/) was used in this project and installed using these [instructions](https://soupday.github.io/cc_unity_tools/installation.html). The releases for URP and 3D pipelines can also be installed instead and the [newer versions](https://github.com/soupday/CCiC-Unity-Tools) might work as well.

### Setup
1. Clone this repository.
2. Download the [SyDH-Assets.zip](https://tubcloud.tu-berlin.de/s/LaLmZqPngZFBJj5) which contains Assets for SyDH.
3. Unzip SyDH-Assets.zip and copy the files of the Assets folder into the Assets folder of this repo.
4. Add this repository folder to the Unity Hub ("Add" -> "Add project from disk") and start this project.

## 📄 Licence
This work is licensed under a [Creative Commons Attribution-NonCommercial 4.0 International License][cc-by-nc] for non-commercial use only.

[![CC BY-NC 4.0][cc-by-nc-image]][cc-by-nc]

[cc-by-nc]: https://creativecommons.org/licenses/by-nc/4.0/
[cc-by-nc-image]: https://licensebuttons.net/l/by-nc/4.0/88x31.png
[cc-by-nc-shield]: https://img.shields.io/badge/License-CC%20BY--NC%204.0-lightgrey.svg
