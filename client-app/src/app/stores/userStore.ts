import {action, computed, observable, runInAction} from "mobx";
import {IUser, IUserFormValues} from "../models/user";
import agent from "../api/agent";
import {RootStore} from "./rootStore";
import {history} from '../..';

export default class UserStore {

    rootStore: RootStore;

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
    }

    @observable user: IUser | null = null;

    @computed get isLoggedIn() {
        return !!this.user;
    }

    @action login = async (values: IUserFormValues) => {
        try {
            const user = await agent.User.login(values);
            runInAction(() => {
                this.user = user;
                // console.log('current user :', user)
            });

            this.rootStore.commonStore.setToken(user.token);
            this.rootStore.modalStore.closeModal();
            history.push('/activities')

        } catch (error) {
            console.log(error);
            throw error;
        }
    };

    @action register = async (values: IUserFormValues) => {
        try {
            const user = await agent.User.register((values));

            runInAction(() => {
                this.user = user;
            });

            this.rootStore.commonStore.setToken(user.token);
            this.rootStore.modalStore.closeModal();
            history.push('/activities')

        } catch (error) {
            console.log(error);
            throw error;
        }
    };

    @action getUser = async () => {
        try {
            const user = await agent.User.currentUser();
            runInAction(() => {
                this.user = user;
            });
        } catch (e) {
            console.log('Error : ', e);
            throw e
        }
    };


    @action logout = () => {
        this.rootStore.commonStore.setToken(null);
        this.user = null;
        history.push('/')
    };


}