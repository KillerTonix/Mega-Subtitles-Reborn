local SyncCsToReaper = {}

function SyncCsToReaper.Sync()
    -- Check if the project is currently playing --
    local cursor_pos = 0
    if reaper.GetPlayState() ~= 0 then -- If the project is playing
        cursor_pos = reaper.GetPlayPosition() -- Use playback cursor if the project is playing
    else
        cursor_pos = reaper.GetCursorPosition() -- Use edit cursor if the project is not playing
    end

    local regionPos = nil
    local closest_region_dist = math.huge

    for i = 0, CountOfProjectMarkers - 1 do
        local retval, isrgn, pos, rgnend, name, idx = reaper.EnumProjectMarkers(i)
        if isrgn then
            if cursor_pos >= pos and cursor_pos <= rgnend then
                regionPos = pos -- found region under cursor
                break
            else
                -- Find the closest region
                local dist = math.min(math.abs(cursor_pos - pos), math.abs(cursor_pos - rgnend))
                if dist < closest_region_dist then
                    closest_region_dist = dist
                    regionPos = pos
                end
            end
        end
    end   
    WriteSyncFile(SyncForCSharpFilePath, regionPos) -- Write the cursor position to the sync file
end

function WriteSyncFile(filename, content)
    local file = io.open(filename, "w")
	file:write(content)
	file:close()
end

return SyncCsToReaper
