#include <efi.h>
#include <efilib.h>
EFI_STATUS efi_main (EFI_HANDLE image, EFI_SYSTEM_TABLE *systab)
{
	InitializeLib(image, systab);
	UINTN mms,mk,ds,dv;
	int i;
	int s;
	EFI_MEMORY_DESCRIPTOR *mm;
	EFI_STATUS state;
	
	state = uefi_call_wrapper(systab->BootServices->GetMemoryMap,
				5,
				&mms,
				mm,
				&mk,
				&ds,
				&dv);
	if(state == EFI_BUFFER_TOO_SMALL)
	if(
		uefi_call_wrapper(systab->BootServices->AllocatePool,
				3,
				EfiLoaderData,
				mms,
				((void*)&mm))
		== EFI_SUCCESS
		)
	{	
	uefi_call_wrapper(systab->BootServices->GetMemoryMap,
				5,
				&mms,
				mm,
				&mk,
				&ds,
				&dv);
	s=0;
	for (i=0;i<mms;i++)
		if((mm[i].Type == EfiBootServicesCode)
		||(mm[i].Type == EfiBootServicesData)
		||(mm[i].Type == EfiConventionalMemory)
		||(mm[i].Type == EfiACPIReclaimMemory)
		)s+=mm[i].NumberOfPages;
		
	Print(L"%d bytes aviable\r\n",s<<12);
	}
	else Print (L"Allocation memory fail\r\n");
	else Print (L"Error \r\n");
	return EFI_SUCCESS;
}
