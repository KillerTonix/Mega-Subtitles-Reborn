local SyncWithCSharp = {}

function SyncWithCSharp.Sync()	
	if CurrentPosition ~= reaper.GetCursorPosition() then -- Check if CurrentPosition is set and not equal to the current cursor position
		reaper.SetEditCurPos(tonumber(CurrentPosition), true, false) -- Set the edit cursor position to CurrentPosition
	end	
end

return SyncWithCSharp
