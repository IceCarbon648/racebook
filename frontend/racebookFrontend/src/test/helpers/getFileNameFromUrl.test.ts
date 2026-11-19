import { describe, it, expect } from 'vitest';
import { getFileNameFromUrl } from '../../helpers/getFileNameFromUrl';

const CLOUDINARY_BASE = 'https://res.cloudinary.com/racebook/raw/upload/v1762000000/Mods';

describe('getFileNameFromUrl', () => {
    it('returns null when there is no url', () => {
        expect(getFileNameFromUrl(null)).toBeNull();
        expect(getFileNameFromUrl(undefined)).toBeNull();
        expect(getFileNameFromUrl('')).toBeNull();
    });

    it('returns the file name from a Cloudinary url', () => {
        expect(getFileNameFromUrl(`${CLOUDINARY_BASE}/o04a2toyr9v2weqm3tqv.tpf`))
            .toBe('o04a2toyr9v2weqm3tqv.tpf');
    });

    it('ignores a query string', () => {
        expect(getFileNameFromUrl(`${CLOUDINARY_BASE}/skyline.tpf?_a=BAMCkGeK`))
            .toBe('skyline.tpf');
    });

    it('decodes a percent encoded file name', () => {
        expect(getFileNameFromUrl(`${CLOUDINARY_BASE}/neon%20skyline.tpf`))
            .toBe('neon skyline.tpf');
    });

    it('returns null when the url has no file name', () => {
        expect(getFileNameFromUrl('https://res.cloudinary.com')).toBeNull();
        expect(getFileNameFromUrl('https://res.cloudinary.com/')).toBeNull();
    });

    it('falls back to string handling when the url is not absolute', () => {
        expect(getFileNameFromUrl('Mods/skyline.tpf')).toBe('skyline.tpf');
        expect(getFileNameFromUrl('Mods/skyline.tpf?v=2')).toBe('skyline.tpf');
    });
});