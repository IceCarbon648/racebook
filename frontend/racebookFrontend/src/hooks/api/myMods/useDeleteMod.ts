import { useMutation } from "@tanstack/react-query";
import { deleteMod as deleteModService } from "../../../services";
import { useInvalidateMods } from "./useInvalidateMods";

export const useDeleteMod = () => {
  return useMutation({
    mutationFn: deleteModService,
    onSuccess: useInvalidateMods(),
  });
};
