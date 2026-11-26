# Known Issues

1. Edit method in mod service requires a refactor. Currently old files are being deleted as soon as new files are uploaded successfully. This is an issue of the file uploads and the second does not. Deletion of old files should happen last once all changes are made without issue
2. Performance drop when navigating between pages that use the fluid cursor effect. Not noticeable with the first few navigations but become apparent after a few more navigations
3. Mod cards flicker on non-chromium browsers when hovering and pointer is moving
4. Chrome does not seem to be able to display images
