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
		if commandType == "WithOutColor" then
			ColorizeAllTracks.UnColorize()
		elseif commandType == "WithColor" then
			ColorizeAllTracks.colorize(entries, partly)
		end
	end
end

return ParseDeleteCreate