import {RootStore} from "./rootStore";
import {action, computed, observable, reaction, runInAction} from "mobx";
import {IProfile, IPhoto, IUserActivity} from "../models/profile";
import agent from "../api/agent";
import {toast} from "react-toastify";

export default class ProfileStore {
    rootStore: RootStore;

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;

        // adding reaction for 
        reaction(() => this.activeTab, activeTab => {
            if (activeTab === 3 || activeTab === 4) {
                const predicate = activeTab === 3 ? 'followers' : 'following';
                this.loadingFollowings(predicate);
            } else {
                this.followings = [];
            }
        });
    }

    @observable profile: IProfile | null = null;
    @observable loadingProfile: boolean = true;
    @observable uploadingPhoto: boolean = false;
    @observable loading: boolean = false;
    @observable followings: IProfile[] = [];
    @observable activeTab: number = 0;
    @observable userActivities: IUserActivity[] = [];
    @observable loadingActivities = false;


    @computed get isCurrentUser() {
        if (this.rootStore.userStore.user && this.profile) {
            return this.rootStore.userStore.user.username === this.profile.username;
        } else {
            return false;
        }
    }

    @action loadUserActivities = async (username: string, predicate?: string) => {
        this.loadingActivities = true;
        try {
            const activities = await agent.Profile.listActivities(username, predicate!);
            runInAction(()=>{
                this.userActivities = activities;
                this.loadingActivities = false;
            });

        } catch (error) {
            toast.error('Error loading user activity');
            this.loadingActivities = false;
        }
    };


    @action setActiveTab = (activeIndex: number) => {
        this.activeTab = activeIndex;
    };

    @action loadProfile = async (username: string) => {
        this.loadingProfile = true;
        try {
            const profile = await agent.Profile.get(username);
            runInAction(() => {
                this.profile = profile;
                this.loadingProfile = false;
            });

        } catch (error) {

            runInAction(() => {
                this.loadingProfile = false;

            });
            console.log('error happened : ', error)

        }
    };

    @action uploadPhoto = async (file: Blob) => {
        try {
            this.uploadingPhoto = true;
            const photo = await agent.Profile.uploadPhoto(file);
            runInAction(() => {

                if (this.profile) {
                    this.profile.photos.push(photo);
                    if (photo.isMain && this.rootStore.userStore.user) {
                        this.rootStore.userStore.user.image = photo.url;
                        this.profile.image = photo.url;
                    }
                }
                this.uploadingPhoto = false;
            });
        } catch (error) {
            console.log('Error happened in uploading photo')
            console.log(error)
            runInAction(() => {
                toast.error('Problem uploading photo')
                this.uploadingPhoto = false;
            })
        }

    };

    @action setMainPhoto = async (photo: IPhoto) => {
        this.loading = true;
        try {
            await agent.Profile.setMain(photo.id);
            runInAction(() => {
                this.rootStore.userStore.user!.image = photo.url;
                this.profile!.photos.find(a => a.isMain)!.isMain = false;
                this.profile!.photos.find(a => a.id === photo.id)!.isMain = true;
                this.profile!.image = photo.url;
                this.loading = false;
            });
        } catch (error) {
            toast.error('Problem setting photo as main');
            runInAction(() => {
                this.loading = false;
            });
        }
    };

    @action updateProfile = async (profile: Partial<IProfile>) => {
        try {
            await agent.Profile.editProfile(profile);
            runInAction(() => {
                if (profile.displayName !== this.rootStore.userStore.user!.displayName) {
                    this.rootStore.userStore.user!.displayName = profile.displayName!;
                    this.profile = {...this.profile!, ...profile};
                }
            });
        } catch (error) {
            toast.error("Problem updating profile");
        }
    };


    @action deletePhoto = async (photo: IPhoto) => {
        this.loading = true;
        try {
            await agent.Profile.deletePhoto(photo.id);
            runInAction(() => {
                this.profile!.photos = this.profile!.photos.filter(
                    a => a.id !== photo.id
                );
                this.loading = false;
            });
        } catch (error) {
            toast.error('Problem deleting the photo');
            runInAction(() => {
                this.loading = false;
            });
        }
    };


    // follow a user 
    @action follow = async (username: string) => {
        this.loading = true;
        try {
            await agent.Profile.follow(username);
            runInAction(() => {
                this.profile!.following = true;
                this.profile!.followingCount++;
                this.loading = false;

            });

        } catch (error) {
            toast.error('Problem following user !!')
            runInAction(() => {
                this.loading = false;
            });
        }
    };


    // un-follow a user
    @action unfollow = async (username: string) => {
        this.loading = true;
        try {

            await agent.Profile.unfollow(username);
            runInAction(() => {
                this.profile!.following = false;
                this.profile!.followingCount--;
                this.loading = false;

            });

        } catch (error) {
            toast.error('Problem un-following user !!')
            runInAction(() => {
                this.loading = false;
            });
        }
    };

    // loading followings
    @action loadingFollowings = async (predicate: string) => {
        this.loading = true;
        try {

            const profile = await agent.Profile.listFollowings(this.profile!.username, predicate);

            runInAction(() => {
                this.followings = profile;
                this.loading = false;
            });

        } catch (error) {
            console.log(error);
            toast.error('Problem getting followers');
            runInAction(() => {
                this.loading = false;
            });

        }
    };

} 