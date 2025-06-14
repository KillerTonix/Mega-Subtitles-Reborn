-- CreateRegions.lua
local CreateRegions = {}

local GetUniqueActorsFromCache = require("GetUniqueActorsFromCache")

function CreateRegions.addMarkers(subtitleEntries)		
		for actor, entries in pairs(subtitleEntries) do -- Iterate through each actor and their entries
			for i, entry in ipairs(entries) do -- For each entry, add a project marker
				reaper.AddProjectMarker2(ProjectID, true, tonumber(entry.Start), tonumber(entry.End), entry.Text, -1, tonumber(entry.Color)) -- Add a project marker with the start time, end time, text, and color
			end
		end	
    CountOfProjectMarkers = reaper.CountProjectMarkers(ProjectID) -- Get the count of project markers in the current project	   
end

return CreateRegions
