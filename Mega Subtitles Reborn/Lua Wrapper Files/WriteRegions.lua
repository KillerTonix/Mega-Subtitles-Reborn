-- WriteRegions.lua
local WriteRegions = {}
json = require "json"

function save_json(filename, content)
	local file = io.open(filename, "w")
	file:write(content)
	file:close()
end


function WriteRegions.remainingRegions()	
	local data = {} -- Initialize an empty table to hold the region data
	local cacheFilePath = CacheFolderPath .. "\\" .. CurrentProjectName .. "\\" .. CurrentProjectName .. "_CacheFromReaper.json"
    
	for i = 0, CountOfProjectMarkers - 1 do			
		local retval, is_region, start_time, end_time, text, markrgn_index, color = reaper.EnumProjectMarkers3(ProjectID, i)
		
		if is_region then
		table.insert(data, {
			Start = start_time,
			End = end_time,
			Text = text or ""
		})
		end
	end	
	
	local jsonString = json.encode(data) -- Convert the data table to a JSON string
	save_json(cacheFilePath, jsonString) -- Save the JSON string to the specified file
end

return WriteRegions