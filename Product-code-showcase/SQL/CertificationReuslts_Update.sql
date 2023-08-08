

ALTER Proc [dbo].[CertificationResults_Update]

		 @CertificationId	     int
		,@IsPhysicalCompleted	     bit
		,@IsBackgroundCheckCompleted bit
		,@IsTestCompleted	     bit				
		,@TestInstanceId             int = NULL
		,@Score		             decimal(5,2) = NULL
		,@IsFitnessTestCompleted     bit
		,@IsClinicAttended	     bit
		,@IsActive	       	     bit
		,@UserId		     int
		,@ModifiedBy		     int
		,@Id                         int OUTPUT			
		
AS

/* 

		 Declare  @Id                int = 728
		,@CertificationId 	     int = 74  --FK
		,@IsPhysicalCompleted	     bit = 1
		,@IsBackgroundCheckCompleted bit = 1
		,@IsTestCompleted	     bit = 1	
		,@TestInstanceId	     int = 1
		,@Score		             decimal(5,2) = 6.6
		,@IsFitnessTestCompleted     bit = 1  
		,@IsClinicAttended	     bit = 1
		,@IsActive	             bit = 1
		,@UserId                     int = 6
		,@ModifiedBy		     int = 6  --FK
		

EXECUTE	[dbo].[CertificationResults_Update]

         @CertificationId		     
		,@IsPhysicalCompleted	     
		,@IsBackgroundCheckCompleted 
		,@IsTestCompleted			 				
		,@TestInstanceId		     	
		,@Score		               
		,@IsFitnessTestCompleted     
		,@IsClinicAttended		     
		,@IsActive			
		,@UserId
		,@ModifiedBy				 
		,@Id Output
	

	Select *
	From dbo.CertificationResults

*/

Begin
	
Declare @DateModified datetime2 = GETUTCDATE()

DECLARE  @IsPhysicalRequired BIT
		,@IsBackgroundCheckRequired BIT
		,@IsTestRequired BIT
		,@IsFitnessTestRequired BIT
		,@IsClinicRequired BIT

SELECT   @IsPhysicalRequired = IsPhysicalRequired
		,@IsBackgroundCheckRequired = IsBackgroundCheckRequired
		,@IsTestRequired = IsTestRequired
		,@IsFitnessTestRequired = IsFitnessTestRequired
		,@IsClinicRequired = IsClinicRequired
		
FROM [dbo].[Certifications]

WHERE Id = @CertificationId

DECLARE @IsAllCompleted BIT = CASE WHEN (@IsPhysicalRequired = 0 OR @IsPhysicalRequired = @IsPhysicalCompleted)
									  AND (@IsBackgroundCheckRequired = 0 OR @IsBackgroundCheckRequired = @IsBackgroundCheckCompleted) 
									  AND (@IsTestRequired = 0 OR @IsTestRequired = @IsTestRequired)
									  AND (@IsFitnessTestRequired = 0 OR @IsFitnessTestRequired = @IsFitnessTestCompleted)
									  AND (@IsClinicRequired = 0 OR @IsClinicRequired = @IsClinicAttended )
									  THEN 1
									  ELSE 0
									  END
									  
SELECT @IsAllCompleted

Update [dbo].[CertificationResults]    

Set      [CertificationId]		      =@CertificationId	
		,[IsPhysicalCompleted]	      =@IsPhysicalCompleted	
		,[IsBackgroundCheckCompleted] =@IsBackgroundCheckCompleted
		,[IsTestCompleted]	      =@IsTestCompleted		
		,[TestInstanceId]	      =(CASE WHEN @TestInstanceId IS NOT NULL THEN @TestInstanceId ELSE [TestInstanceId] END)	
		,[Score]		      =(CASE WHEN @Score IS NOT NULL THEN @Score ELSE [Score] END)
		,[IsFitnessTestCompleted]     =@IsFitnessTestCompleted 
		,[IsClinicAttended]	      =@IsClinicAttended	
		,[IsActive]		      =@IsActive	
		,[UserId]		      =@UserId	
	    ,[ModifiedBy]                     =@ModifiedBy
		,[DateModified]               =@DateModified
		,[IsCompleted]                =@IsAllCompleted
		Where Id = @Id
End
