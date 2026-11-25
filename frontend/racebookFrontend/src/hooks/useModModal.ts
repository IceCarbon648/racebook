import { useState } from "react";
import type { SubmitEvent } from "react";
import type { ModModalProps } from "../components/organisms/ModModal/index.types";
import { getResponseMessage } from "../utils/getResponseMessage";
import { notify } from "../utils/notify";
import { validateModForm } from "../helpers/validateModForm";

export const useModModal = (
  { mode, mod, onSubmit, onClose }: ModModalProps,
  categories: string[],
) => {
  const isEditing = mode === "edit";
  const existingMod = isEditing ? mod : undefined;

  const [type, setType] = useState(existingMod?.type ?? "");
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [error, setError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const handleSubmit = async (event: SubmitEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);

    const formData = new FormData(event.currentTarget);
    formData.set("type", type);

    const errors = await validateModForm(formData, type, isEditing, categories);

    setFieldErrors(errors ?? {});
    if (errors) {
      return;
    }

    setIsSaving(true);

    try {
      await onSubmit(formData);
      notify.success(isEditing ? "Changes saved" : "Mod uploaded");
      onClose();
    } catch (err) {
      setError(getResponseMessage(err));
    } finally {
      setIsSaving(false);
    }
  };

  return {
    existingMod,
    isEditing,
    type,
    setType,
    fieldErrors,
    error,
    isSaving,
    handleSubmit,
  };
};
