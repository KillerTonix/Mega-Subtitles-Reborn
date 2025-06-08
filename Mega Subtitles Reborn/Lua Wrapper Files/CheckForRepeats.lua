-- CheckForRepeats.lua
local CheckForRepeats = {}

local WriteRegions = require("WriteRegions")

function CheckForRepeats.check()
    local project_ID, _ = reaper.EnumProjects(-1, "")
    local retval, num_markers, num_regions = reaper.CountProjectMarkers(project_ID)   
    local threshold_percentage = 55
    reaper.PreventUIRefresh(1)

    for i = num_regions - 1, 0, -1 do
        local retval, is_region, position, region_end, name, idx = reaper.EnumProjectMarkers(i)
		

        local region_original_length = region_end - position
        local threshold_length = (threshold_percentage / 100) * region_original_length
        
		position = position + 0.5
		region_end = region_end - 0.5
		local region_length = region_end - position
		
        if is_region then            
            -- Count the items under the region
            local item_count = 0
            local num_tracks = reaper.CountTracks(0)
            
            -- Find last region end position (project duration)
            local project_duration = 0
            for j = 0, num_regions - 1 do
                local _, isrgn, _, region_end = reaper.EnumProjectMarkers(j)
                if isrgn and region_end > project_duration then
                    project_duration = region_end
                end
            end
            
            -- Calculate threshold length (30% of project duration)
            local duration_threshold = project_duration * 0.3
            
            for track_idx = 0, num_tracks - 1 do 
                local track = reaper.GetTrack(0, track_idx)
                if track then
                    -- Calculate total length of items in track
                    local track_total_length = 0
                    local num_items = reaper.CountTrackMediaItems(track)
                    
                    for item_idx = 0, num_items - 1 do
                        local item = reaper.GetTrackMediaItem(track, item_idx)
                        track_total_length = track_total_length + reaper.GetMediaItemInfo_Value(item, "D_LENGTH")
                    end
                    
                    -- Only process tracks with total item length less than 30% of project duration
                    if track_total_length < duration_threshold then
                        for item_idx = 0, num_items - 1 do
                            local item = reaper.GetTrackMediaItem(track, item_idx)
                            local item_start = reaper.GetMediaItemInfo_Value(item, "D_POSITION")
                            local item_length = reaper.GetMediaItemInfo_Value(item, "D_LENGTH")
                            local item_end = item_start + item_length
                            
                            -- Check if item is within region boundaries
                            if item_start < region_end and item_end > position and item_length >= threshold_length then
                                item_count = item_count + 1
                                break
                            end
                        end
                    end
                end
            end
            
            -- Delete region if 1 or fewer items are found
            if item_count <= 1 then
                reaper.DeleteProjectMarker(0, idx, is_region)
            end
        end
    end
    reaper.PreventUIRefresh(-1)
    reaper.UpdateArrange()
    WriteRegions.remainingRegions()

end


return CheckForRepeats