-- CreateRegions.lua
local CreateRegions = {}

local GetUniqueActorsFromCache = require("GetUniqueActorsFromCache")

function CreateRegions.addMarkers(subtitleEntries, partly)	
	if partly then
		local uniqueNameList = GetUniqueActorsFromCache.getActors(subtitleEntries) -- Get unique actors from the subtitle entries

		for actor, entries in pairs(subtitleEntries) do -- Check if the actor is in the unique list
			local isUniqueActor = false -- Initialize a flag to check if the actor is unique
			for _, uniqueActor in ipairs(uniqueNameList) do -- Iterate through the unique actors list
				if actor == uniqueActor then -- If the actor matches a unique actor
					isUniqueActor = true -- Set the flag to true
					break
				end
			end

			for i, entry in ipairs(entries) do -- For each entry, add a project marker
				if isUniqueActor then -- If the actor is unique, set the color to the entry's color
					reaper.AddProjectMarker2(ProjectID, true, entry.Start, entry.End, entry.Text, -1, entry.Color) -- If the actor is unique, set the color to the entry's color
				else
					reaper.AddProjectMarker2(ProjectID, true, entry.Start, entry.End, entry.Text, -1, 0) -- If the actor is not unique, set color to 0 (no color)
				end
			end
		end
	else
		for actor, entries in pairs(subtitleEntries) do -- Iterate through each actor and their entries
			for i, entry in ipairs(entries) do -- For each entry, add a project marker
				reaper.AddProjectMarker2(ProjectID, true, tonumber(entry.Start), tonumber(entry.End), entry.Text, -1, tonumber(entry.Color)) -- Add a project marker with the start time, end time, text, and color
			end
		end
	end	
    CountOfProjectMarkers = reaper.CountProjectMarkers(ProjectID) -- Get the count of project markers in the current project
	   
end

return CreateRegions
