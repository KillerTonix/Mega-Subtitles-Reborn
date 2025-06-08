-- CheckForMissing.lua
local CheckForMissing = {}

local WriteRegions = require("WriteRegions")

local THRESHOLD_PERCENTAGE = 0.30  -- 30% as decimal
local RED_COLOR = reaper.ColorToNative(255, 0, 0) | 0x1000000


function CheckForMissing.checkingAndColoring()
    local tracks = {}

    local project_duration = 0
    for i = 0, CountOfProjectMarkers - 1 do
        local _, isrgn, _, region_end = reaper.EnumProjectMarkers(i)
        if isrgn and region_end > project_duration then
            project_duration = region_end
        end
    end

    -- Calculate threshold length for tracks
    local track_threshold = project_duration * THRESHOLD_PERCENTAGE
    
    for i = 0, NumerOfTracks - 1 do
        local track = reaper.GetTrack(ProjectID, i)
        local track_total_length = 0
        local num_items = reaper.CountTrackMediaItems(track)
        
        -- Calculate total length of items in track
        for j = 0, num_items - 1 do
            local item = reaper.GetTrackMediaItem(track, j)
            track_total_length = reaper.GetMediaItemInfo_Value(item, "D_LENGTH")
        end
        
        -- Add track if total items length is less than threshold
        if track_total_length < track_threshold then
            table.insert(tracks, track)
        end
    end
    
    -- Process regions
    for i = 0, CountOfProjectMarkers - 1 do
        local _, isrgn, region_start, region_end, _, region_idx = reaper.EnumProjectMarkers(i)
        
        if isrgn then
            local region_length = region_end - region_start
            local total_length = getItemsInRegion(region_start, region_end, tracks)
            local threshold_length = region_length * THRESHOLD_PERCENTAGE
            
            if total_length < threshold_length then
                reaper.SetProjectMarkerByIndex(ProjectID, i, isrgn, region_start, region_end, region_idx, "", RED_COLOR)
            else
                 reaper.defer(function() DeleteProjectMarker(ProjectID, region_idx) end)
            end
        end
    end

    -- Defer writing regions
    local start_time = reaper.time_precise()
    local function timer()
        if reaper.time_precise() - start_time >= 5 then
            WriteRegions.remainingRegions()
        else
            reaper.defer(timer)
        end
    end
    reaper.defer(timer)
end

function DeleteProjectMarker(ProjectID, region_idx)
    reaper.DeleteProjectMarker(ProjectID, region_idx, true)
end 

function getItemsInRegion(region_start, region_end, tracks)
    local total_length = 0
    
    for _, track in ipairs(tracks) do
        local num_items = reaper.CountTrackMediaItems(track)
        
        for j = 0, num_items - 1 do
            local item = reaper.GetTrackMediaItem(track, j)
            local item_start = reaper.GetMediaItemInfo_Value(item, "D_POSITION")
            local item_length = reaper.GetMediaItemInfo_Value(item, "D_LENGTH")
            local item_end = item_start + item_length

            if item_start < region_end and item_end > region_start then
                -- Calculate overlap
                local overlap_start = math.max(item_start, region_start)
                local overlap_end = math.min(item_end, region_end)
                total_length = total_length + (overlap_end - overlap_start)
            end
        end
    end
    return total_length
end

return CheckForMissing