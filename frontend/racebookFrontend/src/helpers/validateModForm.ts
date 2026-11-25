import { modEditSchema, modUploadSchema, validate } from "../validation";

export const validateModForm = (
  formData: FormData,
  type: string,
  isEditing: boolean,
  categories: string[],
) =>
  validate(
    isEditing ? modEditSchema(categories) : modUploadSchema(categories),
    {
      title: formData.get("title"),
      type,
      description: formData.get("description"),
      modFile: formData.get("modFile"),
      previewImage: formData.get("previewImage"),
    },
  );
