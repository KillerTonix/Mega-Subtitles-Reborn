-- CheckForMissing.lua
local CheckForMissing = {}

local WriteRegions = require("WriteRegions")

local THRESHOLD_PERCENTAGE = 0.30  -- 30% as decimal
local RED_COLOR = reaper.ColorToNative(255, 0, 0) | 0x1000000


function CheckForMissing.checkingAndColoring()
    local ProjectID = 0
    local tracks = {}

    local project_duration = 0 -- Get the duration of the project
    for i = 0, CountOfProjectMarkers - 1 do -- Loop through all project markers
        local _, isrgn, _, region_end = reaper.EnumProjectMarkers(i) -- Get project markers
        if isrgn and region_end > project_duration then -- Check if the marker is a region and if its end time is greater than the current project duration
            project_duration = region_end -- Update project duration
        end
    end

    local track_threshold = project_duration * THRESHOLD_PERCENTAGE -- Calculate the threshold for track item length

    for i = 0, NumerOfTracks - 1 do -- Loop through all tracks in the project
        local track = reaper.GetTrack(ProjectID, i) -- Get the track by index
        local total_length = 0 -- Initialize total length of items in the track
        local num_items = reaper.CountTrackMediaItems(track) -- Count the number of media items in the track
        for j = 0, num_items - 1 do -- Loop through all media items in the track
            local item = reaper.GetTrackMediaItem(track, j) -- Get the media item by index
            total_length = total_length + reaper.GetMediaItemInfo_Value(item, "D_LENGTH") -- Get the length of the media item and add it to the total length
        end
        if total_length < track_threshold then -- Check if the total length of items in the track is less than the threshold
            table.insert(tracks, track) -- If so, add the track to the list of tracks to be processed
        end
    end

    local cached_items = cacheTrackItems(tracks) -- Cache the media items of the tracks

    local regions = {} -- Initialize an empty table to store regions
    for i = 0, CountOfProjectMarkers - 1 do -- Loop through all project markers
        local _, isrgn, region_start, region_end, name, region_idx = reaper.EnumProjectMarkers(i) -- Get project markers

        if isrgn then -- Check if the marker is a region
            table.insert(regions, {
                region_idx = region_idx,
                start = region_start,
                end_ = region_end,
                name = name
            })
        end
    end

    local index = 0 -- Initialize index for setting project markers
    for j, region in ipairs(regions) do -- Loop through all regions
        local region_length = region.end_ - region.start -- Calculate the length of the region
        local total_length = getItemsInRegion(region.start, region.end_, cached_items) -- Get the total length of items in the region using cached items
        local threshold_length = region_length * THRESHOLD_PERCENTAGE -- Calculate the threshold length for the region

        if total_length < threshold_length then -- Check if the total length of items in the region is less than the threshold length
            reaper.SetProjectMarkerByIndex(ProjectID, index, true, region.start, region.end_,region.region_idx, region.name, RED_COLOR) -- Set the project marker with the RED_COLOR
            index = index + 1 -- Increment the index for the next project marker
        else
            reaper.DeleteProjectMarker(ProjectID, region.region_idx, true) -- Delete the project marker if the total length of items in the region is greater than or equal to the threshold length
        end
    end

    WriteRegions.remainingRegions() -- Write the remaining regions to the project
end

-- Caches media items of each track into a table for faster access
function cacheTrackItems(tracks)
    local cached = {}
    for _, track in ipairs(tracks) do
        local items = {}
        local num_items = reaper.CountTrackMediaItems(track)
        for j = 0, num_items - 1 do
            local item = reaper.GetTrackMediaItem(track, j)
            local item_start = reaper.GetMediaItemInfo_Value(item, "D_POSITION")
            local item_length = reaper.GetMediaItemInfo_Value(item, "D_LENGTH")
            table.insert(items, {start = item_start, length = item_length})
        end
        cached[track] = items
    end
    return cached
end

-- Returns the total length of item content within a region
function getItemsInRegion(region_start, region_end, cached_items)
    local total_length = 0
    for _, items in pairs(cached_items) do
        for _, item in ipairs(items) do
            local item_start = item.start
            local item_end = item.start + item.length
            if item_start < region_end and item_end > region_start then
                local overlap_start = math.max(item_start, region_start)
                local overlap_end = math.min(item_end, region_end)
                total_length = total_length + (overlap_end - overlap_start)
            end
        end
    end
    return total_length
end

return CheckForMissing
