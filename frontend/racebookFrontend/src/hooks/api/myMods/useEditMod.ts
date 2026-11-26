import { useMutation } from "@tanstack/react-query";
import { editMod as editModService } from "../../../services";
import { useInvalidateMods } from "./useInvalidateMods";

export const useEditMod = () => {
  const invalidate = useInvalidateMods();

  return useMutation({
    mutationFn: ({ modId, formData }: { modId: string; formData: FormData }) =>
      editModService(modId, formData),
    onSuccess: invalidate,
  });
};
