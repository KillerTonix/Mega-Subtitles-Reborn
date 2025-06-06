﻿using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using System.IO;
using System.Text;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilitis.FileWriter
{
    public class AssFileWriter
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void WriteAssFile(List<string?> SelectedActors, string AssFilePath, bool isSingleFile)
        {
            var subtitlesData = JsonReader.ReadAssSubtitlesDataJson(GeneralSettings.Default.ProjectCahceJsonPath);

            if (isSingleFile)
            {
                var entries = mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>()
                .Where(e => SelectedActors.Contains(e.Actor)).OrderBy(e => e.Start).ToList();

                using var writer = new StreamWriter(AssFilePath, true, Encoding.UTF8);
                writer.WriteLine(ChangeAssHeader(GetAssHeader(subtitlesData.SubtitlesPath)));

                foreach (var item in entries)
                {
                    writer.WriteLine($"Dialogue: {item.Layer},{item.Start},{item.End},{item.Style},{item.Actor},0,0,0,{item.Effect},{item.Text.Replace(@"\N", Environment.NewLine)}");
                }
            }
            else
            {
                for (int i = 0; i < SelectedActors.Count; i++)
                {
                    string? actor = SelectedActors[i];
                    if (actor == null)
                        return;

                    var entries = mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>().Where(e => e.Actor == actor).ToList();

                    string? OutputPath = Path.Combine(AssFilePath, actor) + ".ass";

                    if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(OutputPath))
                    {
                        var result = MessageBox.Show($"{OutputPath} already exists.\nDo you want to replace it?\nIf press No then will be add '_(1)' after file name.", "Save the subtitles file", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                        if (result == MessageBoxResult.Yes)
                        {
                            File.Delete(OutputPath);
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            OutputPath = Path.Combine(AssFilePath, actor) + "_(1).ass";
                        }
                    }

                    using var writer = new StreamWriter(OutputPath, true, Encoding.UTF8);
                    writer.WriteLine(ChangeAssHeader(GetAssHeader(subtitlesData.SubtitlesPath)));

                    foreach (var item in entries)
                    {
                        writer.WriteLine($"Dialogue: {item.Layer},{item.Start},{item.End},{item.Style},{item.Actor},0,0,0,{item.Effect},{item.Text.Replace(@"\N", Environment.NewLine)}");
                    }

                }
            }
        }


        private static string[] GetAssHeader(string OldSubtitlesFilePath)
        {
            List<string> header = [];
            foreach (string line in File.ReadAllLines(OldSubtitlesFilePath))
            {
                header.Add(line); // Add the line to the list
                if (line.Contains("Format: Layer, Start, End"))
                    break; // Stop reading lines after finding the target line
            }

            return [.. header]; // Convert the list to an array before returning
        }

        private static string ChangeAssHeader(string[] Header)
        {

            for (int i = 0; i < Header.Length; i++)
            {
                if (Header[i].Contains("; Script generated by Aegisub 3.2.2"))
                {
                    Header[i] = "; File generated by Mega Subtitles";
                }
                else if (Header[i].Contains("; http://www.aegisub.org/"))
                {
                    Header[i] = "; Author - Artur Tonoyan\r\n; VK - https://vk.com/aatonoyan\r\n; Discord - arturtonoyan\r\n; Email - artur.tonoyan2012@yandex.ru\r\n; Telegram - @BioWareZ";
                }
            }

            return string.Join("\n", Header);
        }
    }



}
