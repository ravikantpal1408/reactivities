export interface IProfile {
    displayName: any,
    username: any,
    bio: any,
    image: any,
    photos: IPhoto[]
}


export interface IPhoto {
    id: any,
    url: any,
    isMain: boolean,
}