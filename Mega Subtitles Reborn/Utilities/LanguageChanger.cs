using Mega_Subtitles_Reborn.Utilitis.FileReader;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities
{
    public class LanguageChanger
    {

        public static void UpdateLanguage()
        {
            int id = 0;
            if (GeneralSettings.Default.Language == "Русский")
                id = 1;

            var lang = JsonReader.ReadLanguageJson("LanguagesFile.json");
            var mainWindow = (MainWindow)Application.Current.MainWindow;           
          
            mainWindow.DeleteLineListViewContext.Header = lang["DeleteLineListViewContext"][id];
            mainWindow.DublicateLineListViewContext.Header = lang["DublicateLineListViewContext"][id];
            mainWindow.CopyTimingsListViewContext.Header = lang["CopyTimingsListViewContext"][id];
            mainWindow.CopyTextListViewContext.Header = lang["CopyTextListViewContext"][id];
            mainWindow.CopyCommentsListViewContext.Header = lang["CopyCommentsListViewContext"][id];
            mainWindow.ClearCommentsListViewContext.Header = lang["ClearCommentsListViewContext"][id];

            mainWindow.ColorColumn.Header = lang["ColorColumn"][id];
            mainWindow.StartHeader.Header = lang["StartHeader"][id];
            mainWindow.EndHeader.Header = lang["EndHeader"][id];
            mainWindow.ActorColumn.Header = lang["ActorColumn"][id];
            mainWindow.TextHeader.Header = lang["TextHeader"][id];
            mainWindow.CommentsColumn.Header = lang["CommentsColumn"][id];

            mainWindow.ColorColumn.Header = lang["ColorColumn"][id];
            mainWindow.StartHeader.Header = lang["StartHeader"][id];
            mainWindow.EndHeader.Header = lang["EndHeader"][id];
            mainWindow.ActorColumn.Header = lang["ActorColumn"][id];
            mainWindow.TextHeader.Header = lang["TextHeader"][id];
            mainWindow.CommentsColumn.Header = lang["CommentsColumn"][id];

            mainWindow.SetActorColorContext.Header = lang["SetActorColorContext"][id];
            mainWindow.RenameActorContext.Header = lang["RenameActorContext"][id];
            mainWindow.DeleteActorContext.Header = lang["DeleteActorContext"][id];

            mainWindow.ActorsColorColumn.Header = lang["ActorsColorColumn"][id];
            mainWindow.ActorsColumnHeader.Header = lang["ActorsColumnHeader"][id];
            mainWindow.ActorsLineCountHeader.Header = lang["ActorsLineCountHeader"][id];

            mainWindow.SelectAllActorsTB.Text = lang["SelectAllActorsTB"][id];
            mainWindow.ParseSubtitlesTB.Text = lang["ParseSubtitlesTB"][id];
            mainWindow.FilterActorsTB.Text = lang["FilterActorsTB"][id];

            mainWindow.ChangeColorLabel.Content = lang["ChangeColorLabel"][id];
            mainWindow.SetColorBtn.Content = lang["SetColorBtn"][id];
            mainWindow.ColorPickerCancelBtn.Content = lang["ColorPickerCancelBtn"][id];

            mainWindow.RenameActorLabel.Content = lang["RenameActorLabel"][id];
            mainWindow.ActorReanameBtn.Content = lang["ActorReanameBtn"][id];
            mainWindow.ActorReanameCancelBtn.Content = lang["ActorReanameCancelBtn"][id];

            mainWindow.SubtitlesLabel.Content = lang["SubtitlesLabel"][id];
            mainWindow.SelectSubtitlesBtn.Content = lang["SelectSubtitlesBtn"][id];
            mainWindow.SaveSubtitlesTB.Text = lang["SaveSubtitlesTB"][id];
            mainWindow.ClearProjectTB.Text = lang["ClearProjectTB"][id];
            mainWindow.OpenCacheFolderTB.Text = lang["OpenCacheFolderTB"][id];

            mainWindow.CommentsLabel.Content = lang["CommentsLabel"][id];
            mainWindow.ViewOnlyWithCommentsTB.Text = lang["ViewOnlyWithCommentsTB"][id];
            mainWindow.RegionsOnlyWithCommentsTB.Text = lang["RegionsOnlyWithCommentsTB"][id];
            mainWindow.DeleteCommentsTB.Text = lang["DeleteCommentsTB"][id];

            mainWindow.ColorizeSettingsLabel.Content = lang["ColorizeSettingsLabel"][id];
            mainWindow.ColorizeSelectedActorsTB.Text = lang["ColorizeSelectedActorsTB"][id];
            mainWindow.ColorizeSelectedTracksTB.Text = lang["ColorizeSelectedTracksTB"][id];
            mainWindow.ColorizeSelectedActorsCommentsTB.Text = lang["ColorizeSelectedActorsCommentsTB"][id];

            mainWindow.CheckLabel.Content = lang["CheckLabel"][id];
            mainWindow.CheckForMissingTB.Text = lang["CheckForMissingTB"][id];
            mainWindow.CheckForRepeatsTB.Text = lang["CheckForRepeatsTB"][id];
            mainWindow.FindDemoPhrasesTB.Text = lang["FindDemoPhrasesTB"][id];

            mainWindow.CommentsBlockLabel.Content = lang["CommentsBlockLabel"][id];
            mainWindow.ImportCommentsTB.Text = lang["ImportCommentsTB"][id];
            mainWindow.SeparateExportCommentsTB.Text = lang["SeparateExportCommentsTB"][id];
            mainWindow.FullExportCommentsTB.Text = lang["FullExportCommentsTB"][id];

           

          



            int fontsize = 14;
            if (id == 1)
                fontsize = 12;

            mainWindow.SelectAllActorsTB.FontSize= fontsize;
            mainWindow.ParseSubtitlesTB.FontSize= fontsize;
            mainWindow.FilterActorsTB.FontSize= fontsize;

            mainWindow.ChangeColorLabel.FontSize= fontsize;
            mainWindow.SetColorBtn.FontSize= fontsize;
            mainWindow.ColorPickerCancelBtn.FontSize= fontsize;

            mainWindow.RenameActorLabel.FontSize= fontsize;
            mainWindow.ActorReanameBtn.FontSize= fontsize;
            mainWindow.ActorReanameCancelBtn.FontSize= fontsize;

            mainWindow.SubtitlesLabel.FontSize= fontsize;
            mainWindow.SelectSubtitlesBtn.FontSize= fontsize;
            mainWindow.SaveSubtitlesTB.FontSize= fontsize;
            mainWindow.ClearProjectTB.FontSize= fontsize;
            mainWindow.OpenCacheFolderTB.FontSize= fontsize;

            mainWindow.CommentsLabel.FontSize= fontsize;
            mainWindow.ViewOnlyWithCommentsTB.FontSize= fontsize;
            mainWindow.RegionsOnlyWithCommentsTB.FontSize= fontsize;
            mainWindow.DeleteCommentsTB.FontSize= fontsize;

            mainWindow.ColorizeSettingsLabel.FontSize= fontsize;
            mainWindow.ColorizeSelectedActorsTB.FontSize= fontsize;
            mainWindow.ColorizeSelectedTracksTB.FontSize= fontsize;
            mainWindow.ColorizeSelectedActorsCommentsTB.FontSize= fontsize;

            mainWindow.CheckLabel.FontSize= fontsize;
            mainWindow.CheckForMissingTB.FontSize= fontsize;
            mainWindow.CheckForRepeatsTB.FontSize= fontsize;
            mainWindow.FindDemoPhrasesTB.FontSize= fontsize;

            mainWindow.CommentsBlockLabel.FontSize= fontsize;
            mainWindow.ImportCommentsTB.FontSize= fontsize;
            mainWindow.SeparateExportCommentsTB.FontSize= fontsize;
            mainWindow.FullExportCommentsTB.FontSize= fontsize;




            /* if (mainWindow.ActorsList.Items.Count <= 0)
             {
                 mainWindow.DeleteCommentsTB.Text = lang["DeleteCommentsTB2"][id];
            mainWindow.ColorizeSelectedActorsCommentsTB.Text = lang["ColorizeSelectedActorsCommentsTB2"][id];
             }*/


        }
    }
}
