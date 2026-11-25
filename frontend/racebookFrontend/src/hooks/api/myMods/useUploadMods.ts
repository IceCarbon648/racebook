import { useMutation } from "@tanstack/react-query";
import { uploadMod as uploadModService } from "../../../services";
import { useInvalidateMods } from "./useInvalidateMods";

export const useUploadMod = () => {
  const invalidate = useInvalidateMods();

  return useMutation({
    mutationFn: uploadModService,
    onSuccess: invalidate,
  });
};
