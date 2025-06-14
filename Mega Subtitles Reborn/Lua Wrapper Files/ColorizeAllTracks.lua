-- ColorizeAllTracks.lua
local ColorizeAllTracks = {}

local GetUniqueActorsFromCache = require("GetUniqueActorsFromCache")

function ColorizeAllTracks.colorize(subtitleEntries)	
	local uniqueNameList = GetUniqueActorsList(subtitleEntries) -- Get unique actors from the subtitle entries

	for i = 0, NumerOfTracks - 1 do -- Iterate through each track in the project
		track = reaper.GetTrack(ProjectID, i) -- Get the track object for the current index	
		_, track_name = reaper.GetTrackName(track) -- Get the name of the track
	
		for actor, entries in pairs(subtitleEntries) do -- Iterate through each actor and their entries
			for _, uniqueActor in ipairs(uniqueNameList) do -- Iterate through the unique actors list
				if actor == uniqueActor then -- If the actor matches a unique actor
					if string.find(track_name:lower(), actor:lower()) then -- Check if the track name contains the actor's name (case-insensitive)
						reaper.SetTrackColor(track, entries[1].Color) -- Set the track color to the first entry's color for that actor
					end
				end	
			end	
		end	
			
	end
end

function ColorizeAllTracks.UnColorize()
	local color = reaper.GetTrackColor(reaper.GetTrack(ProjectID, 0)) -- Get the color of the first track in the project

	for i = 1, NumerOfTracks - 1 do	 -- Iterate through each track in the project starting from the second track
		track = reaper.GetTrack(ProjectID, i) -- Get the track object for the current index
		reaper.SetMediaTrackInfo_Value(track, 'I_CUSTOMCOLOR', color ) -- Set the track color to the color of the first track
	end
end


-- Function to get sorted keys (actor names)
function GetUniqueActorsList(t)
    local keys = {}
    
    -- Collect all the keys from the table
    for key in pairs(t) do
        table.insert(keys, key)
    end
    
    -- Sort the keys alphabetically
    table.sort(keys)
    
    return keys
end


return ColorizeAllTracks
