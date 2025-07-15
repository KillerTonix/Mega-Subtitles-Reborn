
# 🎬 Save Subtitles Using Template – Help Guide

This feature allows you to generate subtitle files for specific voice actors based on a template file. You can customize the output format and apply additional options like adding noise recording space or a zero timestamp line.

---

## 📝 How to Use This Feature

1. **Select Output Format**
   - 📄 **.ass** (Advanced SubStation Alpha)
   - 📄 **.srt** (SubRip Subtitle)
   - 📄 **.txt** (Plain text)

2. **Check Options (Optional):**
   - ✅ **"Add 10 Seconds for Noise"**  
     Adds a 10-second silent gap (for noise recording) before each actor’s lines in the subtitle.
   - ✅ **"Add Zero Line"**  
     Adds a zero-timestamp line (`0:00:00.00`) at the beginning of the subtitle file.

3. **Click "Save With Template" Button**

4. **Select a Template File**  
   You'll be prompted to choose a `.txt` template file which contains the distribution of actors per dubber.  
   **Format example:**
   ```
   Dubber1: Actor1, Actor2
    Dubber2: Actor3
   ```

4. **Choose Output Folder**  
   You’ll be asked to select a folder where the generated subtitle files will be saved. 

---

## 📂 Template Format Guide

Each line in the template file should follow this format:

```
DubberName: Actor1, Actor2, Actor3
```

- **DubberName** will be used as the output filename.
- **Actors** must match existing actor names in the project.

---

## ⚙️ Output

- One subtitle file is generated per dubber.
- Each file includes only the listed actors' subtitles.
- Files are saved in the selected folder with appropriate extensions (`.ass`, `.srt`, `.txt`).

---

## 🛠 Example

Given this template file:

```
Anna: Alice, Bob
John: Charlie
```

And choosing `.srt` format with both options enabled, the app will create:

- `Anna.srt` → includes Alice and Bob’s lines, with 10 sec noise and a zero line.
- `John.srt` → includes Charlie’s lines, with the same options.

---
