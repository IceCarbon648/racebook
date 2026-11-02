export const getFileNameFromUrl = (url: string | null | undefined): string | null => {
    if (!url) return null;

    try {
        const { pathname } = new URL(url);
        const fileName = pathname.split('/').filter(Boolean).pop();

        return fileName ? decodeURIComponent(fileName) : null;
    } catch {
        const fileName = url.split('?')[0].split('/').filter(Boolean).pop();

        return fileName ? decodeURIComponent(fileName) : null;
    }
};