-- CheckForRepeats.lua
local CheckForRepeats = {}

local WriteRegions = require("WriteRegions")

function CheckForRepeats.check()
    local threshold_percentage = 55 -- Percentage threshold for region length
    reaper.PreventUIRefresh(1) -- Prevent UI refresh for performance
    for i = CountOfProjectMarkers - 1, 0, -1 do -- Iterate backwards to avoid issues with deleting markers
        local retval, is_region, position, region_end, name, idx = reaper.EnumProjectMarkers(i) -- Get marker details
        local region_original_length = region_end - position -- Calculate original region length
        local threshold_length = (threshold_percentage / 100) * region_original_length -- Calculate threshold length based on percentage
        
		position = position + 0.5 -- Adjust position to avoid rounding issues
		region_end = region_end - 0.5 -- Adjust region end to avoid rounding issues
		local region_length = region_end - position  -- Calculate the length of the region
		
        if is_region then            
            local item_count = 0 -- Count the items under the region
                       
            -- Find last region end position (project duration)
            local project_duration = 0 -- Initialize project duration
            for j = 0, CountOfProjectMarkers - 1 do -- Iterate through all markers
                local _, isrgn, _, region_end = reaper.EnumProjectMarkers(j) -- Get marker details
                if isrgn and region_end > project_duration then -- Check if it's a region and if its end is greater than current project duration
                    project_duration = region_end -- Update project duration
                end
            end
                       
            local duration_threshold = project_duration * 0.3 -- Calculate threshold length (30% of project duration)
            
            for track_idx = 0, NumerOfTracks - 1 do  -- Iterate through all tracks
                local track = reaper.GetTrack(0, track_idx) -- Get the track by index
                if track then
                    local track_total_length = 0 -- Calculate total length of items in track
                    local num_items = reaper.CountTrackMediaItems(track) -- Count the number of items in the track
                    
                    for item_idx = 0, num_items - 1 do -- Iterate through all items in the track
                        local item = reaper.GetTrackMediaItem(track, item_idx) -- Get the item by index
                        track_total_length = track_total_length + reaper.GetMediaItemInfo_Value(item, "D_LENGTH") -- Add item length to total length
                    end
                    
                    -- Only process tracks with total item length less than 30% of project duration
                    if track_total_length < duration_threshold then -- Check if total item length is less than threshold
                        for item_idx = 0, num_items - 1 do -- Iterate through all items in the track
                            local item = reaper.GetTrackMediaItem(track, item_idx) -- Get the item by index
                            local item_start = reaper.GetMediaItemInfo_Value(item, "D_POSITION") -- Get item start position
                            local item_length = reaper.GetMediaItemInfo_Value(item, "D_LENGTH") -- Get item length
                            local item_end = item_start + item_length -- Calculate item end position
                            
                            -- Check if item is within region boundaries
                            if item_start < region_end and item_end > position and item_length >= threshold_length then -- Check if item overlaps with region and meets length threshold
                                item_count = item_count + 1 -- Increment item count
                                break
                            end
                        end
                    end
                end
            end
            
            -- Delete region if 1 or fewer items are found
            if item_count <= 1 then -- Check if item count is less than or equal to 1
                reaper.DeleteProjectMarker(ProjectID, idx, is_region) -- Delete the region marker
            end
        end
    end
    reaper.PreventUIRefresh(-1) -- Allow UI refresh
    reaper.UpdateArrange() -- Update the arrangement view
    WriteRegions.remainingRegions() -- Write remaining regions to the project

end


return CheckForRepeats