local SyncWithCSharp = {}

function SyncWithCSharp.Sync()	
	if CurrentPosition ~= reaper.GetCursorPosition() then -- Check if CurrentPosition is set and not equal to the current cursor position
		reaper.SetEditCurPos2(ProjectID, tonumber(CurrentPosition), true, true) -- Set the cursor position to CurrentPosition
	end	
end

return SyncWithCSharp
