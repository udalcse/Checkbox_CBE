IF  EXISTS (SELECT 1 FROM ckbx_Text WHERE TextID = '/pageText/newInvitation.aspx/toTakeTheSurvey')
	UPDATE ckbx_Text SET TextValue = N'to answer a questionnaire' WHERE TextID = '/pageText/newInvitation.aspx/toTakeTheSurvey'
		