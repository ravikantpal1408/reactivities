import React, {useContext, useEffect} from 'react';
import {observer} from "mobx-react-lite";
import {Grid} from "semantic-ui-react";
import ProfileHeader from "./ProfileHeader";
import ProfileContent from "./ProfileContent";
import {RootStoreContext} from "../../app/stores/rootStore";
import {RouteComponentProps} from 'react-router';
import LoadingComponent from "../../app/layout/LoadingComponent";

interface RouteParams {
    username: string;
}

interface IProps extends RouteComponentProps<RouteParams> {
}

const ProfilePage: React.FC<IProps> = ({match}) => {
    const rootStore = useContext(RootStoreContext);

    const {loadingProfile, loadProfile, profile, follow, unfollow, isCurrentUser, loading, setActiveTab} = rootStore.profileStore;

    useEffect(() => {

        loadProfile(match.params.username);

    }, [loadProfile, match]);

    console.log('Profile : ', profile);
    console.log('isCurrentUser : ', isCurrentUser);


    if (loadingProfile) return (<LoadingComponent content='Loading Profile ...'/>);

    return (
        <Grid>
            <Grid.Column
                width={16}>
                <ProfileHeader loading={loading} profile={profile!} isCurrentUser={isCurrentUser} follow={follow}
                               unfollow={unfollow}/>
                <ProfileContent setActiveTab={setActiveTab}/>
            </Grid.Column>
        </Grid>
    )
        ;
};


export default observer(ProfilePage);