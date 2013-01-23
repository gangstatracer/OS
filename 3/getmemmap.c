#include <efi.h>
#include <efilib.h>
; ןונגויראÿ ןנמדנאללא הכÿ UEFI
EFI_STATUS efi_main (EFI_HANDLE image, EFI_SYSTEM_TABLE *systab)
{
	InitializeLib(image, systab);
	UINTN mms=65536;
	int i;
	int s;
	EFI_MEMORY_DESCRIPTOR *mm;
uefi_call_wrapper(systab->BootServices->AllocatePool,
				3,
				EfiLoaderData,
				sizeof(EFI_MEMORY_DESCRIPTOR)*65536,
				((void*)&mm));
	UINTN mk,ds,dv;
	
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
		
	Print(L"%d\r\n",s);
	
	return EFI_SUCCESS;
}
