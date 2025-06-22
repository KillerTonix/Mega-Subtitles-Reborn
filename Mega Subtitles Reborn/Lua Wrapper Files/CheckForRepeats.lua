-- CheckForRepeats.lua
local CheckForRepeats = {}
local WriteRegions = require("WriteRegions")

function CheckForRepeats.check()
    local threshold_percentage = 55 -- Percentage threshold for item length relative to region length
    local duration_threshold_ratio = 0.15 -- Ratio of total track length to consider for deletion
    reaper.PreventUIRefresh(1) -- Prevent UI refresh to speed up processing

    -- 1. Cache all project regions
    local regions = {} -- Table to store regions
    local project_duration = 0 -- Variable to store the maximum region end position

    for i = 0, CountOfProjectMarkers - 1 do -- Loop through all project markers
        local retval, is_region, pos, region_end, name, idx = reaper.EnumProjectMarkers(i) -- Enumerate project markers
        if is_region then -- Check if the marker is a region
            table.insert(regions, {
                position = pos + 0.25,
                region_end = region_end - 0.25,
                name = name,
                idx = idx
            })
            if region_end > project_duration then -- Update project duration if this region ends later
                project_duration = region_end --Update project duration
            end
        end
    end

    -- 2. Cache all items per track
    local duration_threshold = project_duration * duration_threshold_ratio -- Calculate the duration threshold based on the project duration
    local track_cache = {} -- Table to store cached track data

    for track_idx = 0, NumerOfTracks - 1 do -- Loop through all tracks
        local track = reaper.GetTrack(ProjectID, track_idx) -- Get the track by index
        local num_items = reaper.CountTrackMediaItems(track) -- Count the number of media items in the track
        local total_length = 0 -- Variable to store the total length of items in the track
        local items = {} -- Table to store items in the track

        for item_idx = 0, num_items - 1 do -- Loop through all items in the track
            local item = reaper.GetTrackMediaItem(track, item_idx) -- Get the media item by index
            local start_pos = reaper.GetMediaItemInfo_Value(item, "D_POSITION") -- Get the start position of the item
            local length = reaper.GetMediaItemInfo_Value(item, "D_LENGTH") -- Get the length of the item
            total_length = total_length + length -- Update the total length of items in the track
            -- Store item data in the items table
            table.insert(items, {
                start = start_pos,
                length = length,
                end_pos = start_pos + length
            })
        end
        -- Cache the track data
        track_cache[track_idx] = {
            total_length = total_length,
            items = items
        }
    end

    -- 3. Process each region using cached data
    for _, region in ipairs(regions) do -- Loop through each region
        local region_length = region.region_end - region.position -- Calculate the length of the region
        local threshold_length = (threshold_percentage / 100) * region_length -- Calculate the threshold length based on the region length
        local item_count = 0 -- Variable to count items that meet the criteria

        for track_idx, track_data in pairs(track_cache) do -- Loop through each cached track data
            if track_data.total_length < duration_threshold then --Check if the total length of items in the track is below the duration threshold
                for _, item in ipairs(track_data.items) do -- Loop through each item in the track
                    if item.start < region.region_end and item.end_pos > region.position and item.length >= threshold_length then -- Check if the item overlaps with the region and meets the length criteria
                        item_count = item_count + 1 -- Increment the item count
                        break
                    end
                end
            end
        end

        if item_count <= 1 then -- If there is one or no item overlapping with the region
            reaper.DeleteProjectMarker(ProjectID, region.idx, true) -- Delete the region
        end
    end

    reaper.PreventUIRefresh(-1) -- Allow UI refresh after processing
    reaper.UpdateArrange() -- Update the arrangement view
    WriteRegions.remainingRegions() -- Write remaining regions to the project
end

return CheckForRepeats
