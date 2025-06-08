-- ParseDeleteCreate.lua
local ParseDeleteCreate = {}

-- Cache required modules
local ParseCacheFile = require("ParseCacheFile")
local ColorizeAllTracks = require("ColorizeAllTracks")
local DeleteAllRegions = require("DeleteAllRegions")
local CreateRegions = require("CreateRegions")

-- Cache frequently used functions
local preventUI = reaper.PreventUIRefresh
local updateArrange = reaper.UpdateArrange

-- Command handlers table
local TRACK_HANDLERS = {
	WithOutColor = function()
		ColorizeAllTracks.UnColorize()
	end,
	
	WithColor = function(entries, partly)
		ColorizeAllTracks.colorize(entries, partly)
	end
}

function ParseDeleteCreate.regions(commandType, partly)
	local entries = ParseCacheFile.parse(commandType) -- Parse the cache file for regions based on the command type
	if entries then -- If entries are found, proceed with the command
		preventUI(1) -- Prevent UI refresh to improve performance
		DeleteAllRegions.delete() -- Delete all existing regions before creating new ones
		preventUI(-1) -- Allow UI refresh again
		updateArrange() -- Update the arrange view to reflect changes
		
		CreateRegions.addMarkers(entries, partly)
	end
end

function ParseDeleteCreate.tracks(commandType, partly)
	local entries = ParseCacheFile.parse(commandType)
	if entries then
		local handler = TRACK_HANDLERS[commandType]
		if handler then
			handler(entries, partly)
		end
	end
end

return ParseDeleteCreate