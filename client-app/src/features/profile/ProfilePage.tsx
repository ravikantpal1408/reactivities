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

    const {loadingProfile, loadProfile, profile, follow, unfollow, isCurrentUser, loading} = rootStore.profileStore;

    useEffect(() => {

        loadProfile(match.params.username);

    }, [loadProfile, match]);

    if (loadingProfile) return (<LoadingComponent content='Loading Profile ...'/>);

    return (
        <Grid>
            <Grid.Column
                width={16}>
                <ProfileHeader loading={loading} profile={profile!} isCurrentUser={isCurrentUser} follow={follow}
                               unfollow={unfollow}/>
                <ProfileContent/>
            </Grid.Column>
        </Grid>
    )
        ;
};


export default observer(ProfilePage);