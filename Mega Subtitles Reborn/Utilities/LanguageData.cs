using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mega_Subtitles_Reborn.Utilities
{

    public class LanguageData
    {
        public string ColorColumn { get; set; } = "";
        public string StartHeader { get; set; } = "";
        public string EndHeader { get; set; } = "";
        public string ActorColumn { get; set; } = "";
        public string TextHeader { get; set; } = "";
        public string CommentsColumn { get; set; } = "";
        public string SelectAllActorsBtn { get; set; } = ""; 
        public string ParseSubtitlesBtn { get; set; } = ""; 
        public string FilterActorsBtn { get; set; } = "";
        public string ChooseSubtitlesBtn { get; set; } = "";
        public string CommentsExpander { get; set; } = "";
        public string ViewOnlyWithCommentsBtn { get; set; } = "";
        public string RegionsOnlyWithCommentsBtn { get; set; } = "";
        public string DeleteCommentsBtn { get; set; } = "";
        public string DeleteCommentsBtn2 { get; set; } = "";
        public string ColorizeExpander { get; set; } = "";
        public string ColorizeSelectedActorsBtn { get; set; } = "";
        public string ColorizeSelectedTracksBtn { get; set; } = "";
        public string ColorizeSelectedActorsCommentsBtn { get; set; } = "";
        public string ColorizeSelectedActorsCommentsBtn2 { get; set; } = "";
        public string CheckForMissingBtn { get; set; } = "";
        public string ImportCommentsBtn { get; set; } = "";
        public string SeparateExportCommentsBtn { get; set; } = "";
        public string FullExportCommentsBtn { get; set; } = "";
        public string CommentsLabel { get; set; } = "";
        public string SubtitlesLabel { get; set; } = "";
        public string SaveSubtitlesBtn { get; set; } = "";
        public string CheckLabel { get; set; } = "";
        public string SetActorColorContext { get; set; } = "";
        public string RenameActorContext { get; set; } = "";
        public string DeleteActorContext { get; set; } = "";
        public string ChangeColorLabel { get; set; } = "";
        public string SetColorBtn { get; set; } = "";
        public string RenameActorLabel { get; set; } = "";
        public string RenameBtn { get; set; } = "";
        public string CancelBtn { get; set; } = "";
        public string ClearProjectBtn { get; set; } = "";
        public string CheckForRepeatsBtn { get; set; } = "";
        public string OpenCacheFolderBtn { get; set; } = "";
    }

    public class LocalizationRoot
    {
        public LanguageData? En { get; set; }
        public LanguageData? Ru { get; set; }
    }



}