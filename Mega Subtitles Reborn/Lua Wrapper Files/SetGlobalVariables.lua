local SetGlobalVariables = {}

-- <Require List> --
local ClearCommandsFile = require("ClearCommandsFile") -- Clear the commands file before starting
local GetCurrentProjectName = require("GetCurrentProjectName") -- Get the current project name
-- </Require List/> --

function SetGlobalVariables.set()
	CurrentProjectName = GetCurrentProjectName.getName() -- Get the current project name and save it to a file
	ClearCommandsFile.Clear() -- Clear commands file
	ProjectID = reaper.EnumProjects(-1, "") -- Get the current project ID
	NumerOfTracks = reaper.CountTracks(ProjectID) -- Get the number of tracks in the current project
	_, _, CountOfProjectMarkers = reaper.CountProjectMarkers(ProjectID) -- Get the count of project markers in the current project
end





return SetGlobalVariables