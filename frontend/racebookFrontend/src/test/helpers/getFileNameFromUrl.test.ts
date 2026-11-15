import { describe, it, expect } from 'vitest';
import { getFileNameFromUrl } from '../../helpers/getFileNameFromUrl';

const CLOUDINARY_BASE = 'https://res.cloudinary.com/racebook/raw/upload/v1762000000/Mods';

describe('getFileNameFromUrl', () => {
    it('returns null for null', () => {
        expect(getFileNameFromUrl(null)).toBeNull();
    });

    it('returns null for undefined', () => {
        expect(getFileNameFromUrl(undefined)).toBeNull();
    });

    it('returns null for an empty string', () => {
        expect(getFileNameFromUrl('')).toBeNull();
    });

    it('returns the file name from a Cloudinary mod file url', () => {
        expect(getFileNameFromUrl(`${CLOUDINARY_BASE}/o04a2toyr9v2weqm3tqv.tpf`))
            .toBe('o04a2toyr9v2weqm3tqv.tpf');
    });

    it('returns the file name from a Cloudinary image url', () => {
        const url = 'https://res.cloudinary.com/racebook/image/upload/v1762000000/PreviewImages/sbhal99jwoe39i9sejoz.png';

        expect(getFileNameFromUrl(url)).toBe('sbhal99jwoe39i9sejoz.png');
    });

    it('ignores a query string', () => {
        expect(getFileNameFromUrl(`${CLOUDINARY_BASE}/skyline.tpf?_a=BAMCkGeK`))
            .toBe('skyline.tpf');
    });

    it('ignores a fragment', () => {
        expect(getFileNameFromUrl(`${CLOUDINARY_BASE}/skyline.tpf#preview`))
            .toBe('skyline.tpf');
    });

    it('ignores a trailing slash', () => {
        expect(getFileNameFromUrl(`${CLOUDINARY_BASE}/skyline.tpf/`))
            .toBe('skyline.tpf');
    });

    it('decodes a percent encoded file name', () => {
        expect(getFileNameFromUrl(`${CLOUDINARY_BASE}/neon%20skyline.tpf`))
            .toBe('neon skyline.tpf');
    });

    it('returns null when the url has no path', () => {
        expect(getFileNameFromUrl('https://res.cloudinary.com')).toBeNull();
    });

    it('returns null when the url is only a domain with a trailing slash', () => {
        expect(getFileNameFromUrl('https://res.cloudinary.com/')).toBeNull();
    });

    it('falls back to string handling for a relative path', () => {
        expect(getFileNameFromUrl('Mods/skyline.tpf')).toBe('skyline.tpf');
    });

    it('falls back to string handling for a bare file name', () => {
        expect(getFileNameFromUrl('skyline.tpf')).toBe('skyline.tpf');
    });

    it('strips a query string when falling back to string handling', () => {
        expect(getFileNameFromUrl('Mods/skyline.tpf?v=2')).toBe('skyline.tpf');
    });
});