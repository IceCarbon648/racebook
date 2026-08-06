export interface User {
    uid: string;
    username: string;
    amaxUsername?: string;
}

export interface Mod {
    creator: string;
    title: string;
    type: string;
    description: string;
    uploadDate: string;
    editDate: string;
    modFileUrl: string;
    previewImageUrl: string;
}

export interface MyMod {
    modId: string;
    uid: string;
    title: string;
    type: string;
    description: string;
    uploadDate: string;
    editDate: string;
    modFileUrl: string;
    imageUrl: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    email: string;
    username: string;
    password: string;
}

export interface ModUploadRequest {
    title: string;
    type: string;
    description: string;
    modFile: File;
    previewImage: File;
}

export interface ModEditRequest {
    title?: string;
    type?: string;
    description?: string;
    modFile?: File;
    previewImage?: File;
}

export interface AuthResponse {
    token: string;
}